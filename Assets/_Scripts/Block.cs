using UnityEngine;
using UnityEngine.EventSystems;

// ? 블럭 타입 정의
public enum BlockType
{
    Red,
    Blue,
    White
    // 필요시 추가 가능
}

// ? 블럭 동작 스크립트
public class Block : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public BlockType type;                    // 블럭 타입 설정

    [HideInInspector] public int x, y;        // 현재 위치 좌표
    [HideInInspector] public PuzzleBoardManager board;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    // 보드 생성 시 셋업
    public void Setup(int x, int y, PuzzleBoardManager board)
    {
        this.x = x;
        this.y = y;
        this.board = board;
    }

    // 터치/마우스 클릭 시작
    public void OnPointerDown(PointerEventData eventData)
    {
        startTouchPos = eventData.position;
    }

    // 터치/마우스 클릭 끝
    public void OnPointerUp(PointerEventData eventData)
    {
        endTouchPos = eventData.position;
        Vector2 dir = endTouchPos - startTouchPos;

        if (dir.magnitude < 30f) return; // 짧은 터치 무시

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // 좌우
            if (dir.x > 0) board.TrySwap(x, y, x + 1, y);
            else board.TrySwap(x, y, x - 1, y);
        }
        else
        {
            // 상하
            if (dir.y > 0) board.TrySwap(x, y, x, y - 1);
            else board.TrySwap(x, y, x, y + 1);
        }
    }
}
