using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBoardManager : MonoBehaviour
{
    [Header("블럭 프리팹 배열")]
    public GameObject[] blockPrefabs;

    [Header("보드 크기 설정")]
    public int width = 8;
    public int height = 8;
    public float spacing = 5f;

    [Header("부모 패널")]
    public RectTransform boardPanel;

    private BlockType[,] blockTypes;
    private Block[,] blockObjects;

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        RectTransform blockRect = blockPrefabs[0].GetComponent<RectTransform>();
        float blockWidth = blockRect.sizeDelta.x;
        float blockHeight = blockRect.sizeDelta.y;

        float totalWidth = width * (blockWidth + spacing) - spacing;
        float totalHeight = height * (blockHeight + spacing) - spacing;

        Vector2 startPos = new Vector2(
            -totalWidth / 2 + blockWidth / 2,
            totalHeight / 2 - blockHeight / 2
        );

        blockTypes = new BlockType[width, height];
        blockObjects = new Block[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int randomIndex;
                GameObject prefab;
                BlockType selectedType;
                int safety = 0;

                do
                {
                    randomIndex = Random.Range(0, blockPrefabs.Length);
                    prefab = blockPrefabs[randomIndex];
                    selectedType = prefab.GetComponent<Block>().type;

                    safety++;
                    if (safety > 100) break;
                }
                while (
                    (x >= 2 &&
                     blockTypes[x - 1, y] == selectedType &&
                     blockTypes[x - 2, y] == selectedType) ||

                    (y >= 2 &&
                     blockTypes[x, y - 1] == selectedType &&
                     blockTypes[x, y - 2] == selectedType)
                );

                GameObject blockGO = Instantiate(prefab, boardPanel);
                RectTransform rect = blockGO.GetComponent<RectTransform>();

                Vector2 pos = new Vector2(
                    startPos.x + x * (blockWidth + spacing),
                    startPos.y - y * (blockHeight + spacing)
                );
                rect.anchoredPosition = pos;

                Block blockScript = blockGO.GetComponent<Block>();
                blockScript.Setup(x, y, this);

                blockTypes[x, y] = selectedType;
                blockObjects[x, y] = blockScript;
            }
        }
    }

    //  블럭 교체 함수 (매치 판별 포함)
    public void TrySwap(int x1, int y1, int x2, int y2)
    {
        if (x2 < 0 || x2 >= width || y2 < 0 || y2 >= height)
            return;

        Block a = blockObjects[x1, y1];
        Block b = blockObjects[x2, y2];

        // 위치 바꾸기
        Vector2 tempPos = a.GetComponent<RectTransform>().anchoredPosition;
        a.GetComponent<RectTransform>().anchoredPosition = b.GetComponent<RectTransform>().anchoredPosition;
        b.GetComponent<RectTransform>().anchoredPosition = tempPos;

        blockObjects[x1, y1] = b;
        blockObjects[x2, y2] = a;

        a.Setup(x2, y2, this);
        b.Setup(x1, y1, this);

        //  매치 검사
        List<Block> matchA = GetMatchedBlocksAt(x1, y1);
        List<Block> matchB = GetMatchedBlocksAt(x2, y2);

        List<Block> totalMatched = new List<Block>();
        totalMatched.AddRange(matchA);
        totalMatched.AddRange(matchB);

        if (totalMatched.Count >= 3)
        {
            RemoveBlocks(totalMatched);
        }
        else
        {
            // 매치 안 됐으면 원래대로 되돌리기
            tempPos = a.GetComponent<RectTransform>().anchoredPosition;
            a.GetComponent<RectTransform>().anchoredPosition = b.GetComponent<RectTransform>().anchoredPosition;
            b.GetComponent<RectTransform>().anchoredPosition = tempPos;

            blockObjects[x1, y1] = a;
            blockObjects[x2, y2] = b;

            a.Setup(x1, y1, this);
            b.Setup(x2, y2, this);
        }
    }

    //  특정 위치에서 매치된 블럭 찾기
    private List<Block> GetMatchedBlocksAt(int x, int y)
    {
        List<Block> matched = new List<Block>();
        Block center = blockObjects[x, y];
        if (center == null) return matched;

        List<Block> horizontal = new List<Block> { center };
        List<Block> vertical = new List<Block> { center };

        // 좌측
        for (int i = x - 1; i >= 0; i--)
        {
            if (blockObjects[i, y] != null && blockObjects[i, y].type == center.type)
                horizontal.Add(blockObjects[i, y]);
            else break;
        }

        // 우측
        for (int i = x + 1; i < width; i++)
        {
            if (blockObjects[i, y] != null && blockObjects[i, y].type == center.type)
                horizontal.Add(blockObjects[i, y]);
            else break;
        }

        // 위쪽
        for (int i = y - 1; i >= 0; i--)
        {
            if (blockObjects[x, i] != null && blockObjects[x, i].type == center.type)
                vertical.Add(blockObjects[x, i]);
            else break;
        }

        // 아래쪽
        for (int i = y + 1; i < height; i++)
        {
            if (blockObjects[x, i] != null && blockObjects[x, i].type == center.type)
                vertical.Add(blockObjects[x, i]);
            else break;
        }

        if (horizontal.Count >= 3) matched.AddRange(horizontal);
        if (vertical.Count >= 3) matched.AddRange(vertical);

        return matched;
    }

    //  매치된 블럭 제거 (일단 비활성화)
    private void RemoveBlocks(List<Block> matched)
    {
        foreach (Block b in matched)
        {
            int x = b.x;
            int y = b.y;

            b.gameObject.SetActive(false);   // 일단 제거
            blockObjects[x, y] = null;
            blockTypes[x, y] = default;
        }
    }
}
