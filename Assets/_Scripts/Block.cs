using UnityEngine;
using UnityEngine.EventSystems;

// ? �� Ÿ�� ����
public enum BlockType
{
    Red,
    Blue,
    White
    // �ʿ�� �߰� ����
}

// ? �� ���� ��ũ��Ʈ
public class Block : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public BlockType type;                    // �� Ÿ�� ����

    [HideInInspector] public int x, y;        // ���� ��ġ ��ǥ
    [HideInInspector] public PuzzleBoardManager board;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    // ���� ���� �� �¾�
    public void Setup(int x, int y, PuzzleBoardManager board)
    {
        this.x = x;
        this.y = y;
        this.board = board;
    }

    // ��ġ/���콺 Ŭ�� ����
    public void OnPointerDown(PointerEventData eventData)
    {
        startTouchPos = eventData.position;
    }

    // ��ġ/���콺 Ŭ�� ��
    public void OnPointerUp(PointerEventData eventData)
    {
        endTouchPos = eventData.position;
        Vector2 dir = endTouchPos - startTouchPos;

        if (dir.magnitude < 30f) return; // ª�� ��ġ ����

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // �¿�
            if (dir.x > 0) board.TrySwap(x, y, x + 1, y);
            else board.TrySwap(x, y, x - 1, y);
        }
        else
        {
            // ����
            if (dir.y > 0) board.TrySwap(x, y, x, y - 1);
            else board.TrySwap(x, y, x, y + 1);
        }
    }
}
