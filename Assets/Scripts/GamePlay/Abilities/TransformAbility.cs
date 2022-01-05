using Assets.Scripts.GameEvents;
using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TransformAbility
{
	public Transform origin;
	public PlayerController player;

	public TransformConfig conf;

	private float lastUsedTime = float.NegativeInfinity;

	private List<Tile> visitedTiles;
	private Queue<Tile> rimTiles;

	public void Init(PlayerController player) {
		this.player = player;
		visitedTiles = new List<Tile>();
		rimTiles = new Queue<Tile>();
	}

	public void Transform(BiomeType target) {
		Tile tile = player.mapController.GetTileAtPosition(origin.position);

		if (tile != null && tile.GetBiomeType() != BiomeType.NOTTRANSFORMABLE) {
			lastUsedTime = Time.time;
			// transform circle and find rim tiles			
			rimTiles.Enqueue(tile);
			visitedTiles.Add(tile);
			bool noEnergy = false;
			float totalCost = 0;
			for (int depth = 0; depth < conf.radius; depth++) {
				int rimCount = rimTiles.Count;
				for (int i = 0; i < rimCount; i++) {
					Tile next = rimTiles.Dequeue();
					if (!TryMorphTile(next, target, ref totalCost)) 
					{
						noEnergy = true;
						break;
					}

					foreach (Tile ngb in next.GetNeighbours()) {
						if (!visitedTiles.Contains(ngb)) {
							rimTiles.Enqueue(ngb);
							visitedTiles.Add(ngb);
						}
					}
				}
				if (noEnergy)
					break;
			}
			visitedTiles.Clear();

			// deal with rim
			List<Tile> biome = new List<Tile>();
			foreach (Tile rimTile in rimTiles) {
				if (noEnergy)
					break;
				if (rimTile.GetBiomeType() != target) {
					RecursivelySearchBiome(rimTile, biome);
					if (biome.Count <= conf.minBiomeSize) {
						foreach (Tile biomeTile in biome) {
							if (!TryMorphTile(biomeTile, target, ref totalCost))
							{
								noEnergy = true;
								break;
							}
						}
					}

					biome.Clear();
				}
			}
			rimTiles.Clear();
			
			GameEventQueue.QueueEvent(new BiomeTransformedEvent(from: tile.GetBiomeType(), to: target, totalCost, true));
		}
		GameEventQueue.QueueEvent(new BiomeTransformationFailedEvent(invalidTile:true));
	}

	public void Revive()
	{
		Tile tile = player.mapController.GetTileAtPosition(origin.position);
		if (tile != null && tile.GetBiomeType() == BiomeType.DEAD && tile.GetBiomeType() != BiomeType.NOTTRANSFORMABLE)
		{
			lastUsedTime = Time.time;
			tile.Revive();
			player.energy.UseEnergy(conf.energyCost);
			float cost = conf.energyCost;
			foreach (var neigh in tile.GetNeighbours())
            {
				if (neigh.IsDead && player.energy.HasEnough(conf.energyCost))
                {
					neigh.Revive();
					player.energy.UseEnergy(conf.energyCost);
					cost += conf.energyCost;
				}
			}
			GameEventQueue.QueueEvent(new BiomeTransformedEvent(from: BiomeType.DEAD, to: tile.wantedType, cost, true, revive: true));
		}
		else
        {
			GameEventQueue.QueueEvent(new BiomeTransformationFailedEvent(invalidTile: true, revive: true));
		}
	}

	public bool IsReady() {
		if (!player.energy.HasEnough(conf.energyCost))
			return false;
		else
			return Time.time - lastUsedTime > conf.cooldown;
	}

	private void RecursivelySearchBiome(Tile tile, List<Tile> biome) {
		biome.Add(tile);

		if (biome.Count > conf.minBiomeSize) {
			return;
		}

		foreach (Tile ngb in tile.GetNeighbours()) {
			if (ngb.GetBiomeType() == tile.GetBiomeType() && !biome.Contains(ngb)) {
				RecursivelySearchBiome(ngb, biome);
			}
		}
	}

	private bool TryMorphTile(Tile tile, BiomeType target, ref float totalCost) {
		if (tile.GetBiomeType() == target || !tile.CanBeMorphed())
			return true;
		float cost = conf.energyCost;
		if (!tile.IsDead)
			cost *= conf.aliveEnergyCostMultiplier;
        if (player.energy.HasEnough(cost))
        {
			tile.Morph(target, false);
			totalCost += cost;
			player.energy.UseEnergy(cost);
			return true;
		}
		return false;
	}
}
