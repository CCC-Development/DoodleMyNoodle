﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static fixMath;
using Unity.Collections;
using UnityEngine.Serialization;

public class TileHighlightManager : GamePresentationSystem<TileHighlightManager>
{

    [FormerlySerializedAs("HighlightPrefab")]
    [SerializeField] private GameObject _highlightPrefab;

    private List<GameObject> _highlights = new List<GameObject>();
    private Action<GameActionParameterTile.Data> _currentTileSelectionCallback;

    protected override void OnGamePresentationUpdate() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        for (int i = 0; i < _highlights.Count; i++)
        {
            _highlights[i].Destroy();
        }
        _highlights.Clear();
    }

    private void OnHighlightClicked(Vector2 tileHighlightClicked)
    {
        if (_currentTileSelectionCallback != null)
        {
            int2 tile = Helpers.GetTile(tileHighlightClicked);

            GameActionParameterTile.Data TileSelectionData = new GameActionParameterTile.Data(0, tile);

            _currentTileSelectionCallback?.Invoke(TileSelectionData);
            _currentTileSelectionCallback = null;

            HideAll();
        }
    }
    private void CreateHighlight()
    {
        GameObject newHighlight = Instantiate(_highlightPrefab, transform);
        _highlights.Add(newHighlight);

        newHighlight.GetComponent<HighlightClicker>().OnClicked = OnHighlightClicked;
    }

    private void HideAll()
    {
        for (int i = 0; i < _highlights.Count; i++)
        {
            _highlights[i].SetActive(false);
        }
    }

    // TILE PROMPT

    public void AskForSingleTileSelectionAroundPlayer(GameActionParameterTile.Description description, Action<GameActionParameterTile.Data> onSelectCallback)
    {
        _currentTileSelectionCallback = onSelectCallback;

        TileFinder tileFinder = new TileFinder(SimWorld);

        TileFinder.Context context = new TileFinder.Context()
        {
            PawnTile = Cache.LocalPawnTile,
            PawnEntity = Cache.LocalPawn,
        };

        NativeList<int2> tiles = new NativeList<int2>(Allocator.Temp);

        tileFinder.Find(description, context, tiles);

        // Create new highlights (if necessary)
        while (_highlights.Count < tiles.Length)
        {
            CreateHighlight();
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            // Activate Highlight
            _highlights[i].SetActive(true);
            _highlights[i].transform.position = (Vector3)Helpers.GetTileCenter(tiles[i]);
        }

    }

    public void InterruptTileSelectionProcess()
    {
        _currentTileSelectionCallback = null;
        HideAll();
    }
}