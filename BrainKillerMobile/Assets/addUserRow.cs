using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addUserRow : MonoBehaviour
{
    public GameObject userRowPrefab;

    public GameObject firstRowEmpty;
    private Vector3 lastRowPosition;

    private void Start() {
        lastRowPosition = firstRowEmpty.transform.localPosition;
    }

    public void addRow(string id, Color color) {
        GameObject userRow = Instantiate(userRowPrefab, lastRowPosition, Quaternion.identity);
        userRow.transform.SetParent(this.transform, false);
        userRow.GetComponent<setuser>().setUserColor(id, color);

        lastRowPosition.y -= 50;
    }
}
