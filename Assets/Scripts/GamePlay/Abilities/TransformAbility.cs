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
		lastUsedTime = Time.time;

		Tile tile = player.mapController.GetTileAtPosition(origin.position);
		if (tile != null) {
			// transform circle and find rim tiles			
			rimTiles.Enqueue(tile);
			visitedTiles.Add(tile);

			for (int depth = 0; depth < conf.radius; depth++) {
				int rimCount = rimTiles.Count;
				for (int i = 0; i < rimCount; i++) {
					Tile next = rimTiles.Dequeue();
					MorphTile(next, target);
					foreach (Tile ngb in next.GetNeighbours()) {
						if (ngb && !visitedTiles.Contains(ngb)) {
							rimTiles.Enqueue(ngb);
							visitedTiles.Add(ngb);
						}
					}
				}
			}
			visitedTiles.Clear();

			// deal with rim
			List<Tile> biome = new List<Tile>();
			foreach (Tile rimTile in rimTiles) {
				if (rimTile.GetBiomeType() != target) {
					RecursivelySearchBiome(rimTile, biome);
					if (biome.Count <= conf.minBiomeSize) {
						foreach (Tile biomeTile in biome) {
							MorphTile(biomeTile, target);
						}
					}

					biome.Clear();
				}
			}
			rimTiles.Clear();
			
			player.energy.UseEnergy(conf.energyCost);
			GameEventQueue.QueueEvent(new BiomeTransformedEvent(from: tile.GetBiomeType(), to: target, conf.energyCost, true));
		}
		GameEventQueue.QueueEvent(new BiomeTransformationFailedEvent(invalidTile:true));
	}

	public void Revive()
	{
		lastUsedTime = Time.time;
		Tile tile = player.mapController.GetTileAtPosition(origin.position);
		if (tile != null && tile.GetBiomeType() == BiomeType.DEAD)
		{
			GameEventQueue.QueueEvent(new BiomeTransformedEvent(from: BiomeType.DEAD, to: tile.wantedType, conf.energyCost, true));
			tile.Revive();
            int cost = 1;
			foreach(var neigh in tile.GetNeighbours())
            {
				if (neigh.GetBiomeType() == BiomeType.DEAD)
                {
					neigh.Revive();
					cost++;
				}
			}
			player.energy.UseEnergy(conf.energyCost);
		}
		GameEventQueue.QueueEvent(new BiomeTransformationFailedEvent(invalidTile: true, revive: true));
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
			if (ngb && ngb.GetBiomeType() == tile.GetBiomeType() && !biome.Contains(ngb)) {
				RecursivelySearchBiome(ngb, biome);
			}
		}
	}

	private void MorphTile(Tile tile, BiomeType target) {
		tile.Morph(target, false);
		if (target != BiomeType.DEAD) {
			tile.mapController.ReviveTile();
		}
	}
}
