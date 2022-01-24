using Assets.Scripts.GameEvents;
using Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
[Serializable]
public class TransformAbility
{
	public Transform origin;

	public TransformConfig conf;

	public TileHighlighter highlightTilePrefab;


	private float lastUsedTime = float.NegativeInfinity;

	private MapController mapController;
	private EnergyTracker playerEnergy;
	private IObjectPool<TileHighlighter> highlightPool;
	private List<TileHighlighter> highlights = new List<TileHighlighter>();
	public void Init(PlayerController player) {
		highlightPool = new ObjectPool<TileHighlighter>(() => GameObject.Instantiate(highlightTilePrefab), g => g.gameObject.SetActive(true), g => g.gameObject.SetActive(false));
		mapController = player.mapController;
		playerEnergy = player.energy;
	}

	public void Transform(BiomeType target) {
		var tiles = GetTilesToTransform(target, out float cost);
		if(tiles != null)
        {
			lastUsedTime = Time.time;
			tiles.ForEach(t => t.Morph(target, false));
			playerEnergy.UseEnergy(cost);
			GameEventQueue.QueueEvent(new BiomeTransformedEvent(to: target, energyCost: cost, playerOrigin: true));
		}
		else
			GameEventQueue.QueueEvent(new BiomeTransformationFailedEvent(invalidTile: true));
	}

	public void HighlightTransform(BiomeType target)
	{
		var tiles = GetTilesToTransform(target, out float _);
		if (tiles == null)
			return;
		Color hc = mapController.GetBiomeMetadata(target).highlightColor;
		foreach (var t in tiles)
		{
            TileHighlighter h = highlightPool.Get();
			h.transform.position = t.transform.position;
			h.SetHighlightColor(hc);
			highlights.Add(h);
		};
	}

	private List<Tile> GetTilesToTransform(BiomeType target, out float cost)
    {
		cost = 0;
		if (target == BiomeType.UNKNOWN)
			return null;
		
		Tile tile = mapController.GetTileAtPosition(origin.position);

		if (tile == null || !tile.CanBeMorphed())
			return null;

		List<Tile> tilesToTransform = new List<Tile>();

		Queue<Tile> candidates = new Queue<Tile>();
		candidates.Enqueue(tile);
		tile.GetNeighbours().ForEach(candidates.Enqueue);

		float totalCost = 0;
		while (candidates.Count > 0)
		{
			Tile next = candidates.Dequeue();
			if (WillTileMorph(next, target, ref totalCost))
			{
				tilesToTransform.Add(next);
			}
		}
		cost = totalCost;
		return tilesToTransform;
	}

	public void StopHighlightTransform()
	{
		highlights.ForEach(highlightPool.Release);
		highlights.Clear();
	}


	public bool IsReady() {
		if (Time.time - lastUsedTime < conf.cooldown) // CD test
			return false;
		Tile tile = mapController.GetTileAtPosition(origin.position);
		if (tile == null || !tile.CanBeMorphed()) // valid and morphable test
			return false;

		float cost = conf.energyCost;
		if (!tile.IsDead) 
			cost *= conf.aliveEnergyCostMultiplier;

		if (!playerEnergy.HasEnough(cost)) // enough energy test
			return false;

		return true;
	}

	private bool WillTileMorph(Tile tile, BiomeType target, ref float totalCost) {
		if (tile.GetBiomeType() == target || !tile.CanBeMorphed())
			return false;
		float cost = conf.energyCost;
		if (!tile.IsDead)
			cost *= conf.aliveEnergyCostMultiplier;

		float tmpTotalCost = totalCost + cost;

        if (playerEnergy.HasEnough(tmpTotalCost))
        {
			totalCost = tmpTotalCost;
			return true;
		}
		return false;
	}
}
