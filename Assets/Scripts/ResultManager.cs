using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    private Transform _solution;
    
    [SerializeField]
    private TMP_Text _moveCount;

    [SerializeField]
    private TMP_Text _elapsedTime;

    [SerializeField]
    private TMP_Text _visitedCount;

    [SerializeField]
    private TMP_Text _hashSize;

    private void Awake()
    {
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        _solution.gameObject.SetActive(isActive);
    }

    public void SetMoveCount(int count)
    {
        _moveCount.text = count.ToString();
    }

    public void SetElapsedTime(long time)
    {
        _elapsedTime.text = time.ToString();
    }

    public void SetVisitedCount(int count)
    {
        _visitedCount.text = count.ToString();
    }

    public void SetHashSize(int size)
    {
        _hashSize.text = size.ToString();
    }
}