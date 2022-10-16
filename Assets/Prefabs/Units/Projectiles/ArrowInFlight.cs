using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowInFlight : MonoBehaviour
{
    // Start is called before the first frame update
    public Arrow arrow;
    [Tooltip("How high the arc should be, in units")]
    public float arcHeight = 1;
    public float destroySpeed = 1;
    public Collider arrowCollider;

    // Update is called once per frame
    void Update()
    {
        if (!arrow.hasReachedPosition)
        {
            float progressFromStart = (Vector3.Distance(arrow.nextBasePos, arrow.startPos));
            float progressToEndPos = -(Vector3.Distance(arrow.nextBasePos, arrow.targetPos));
            float dist = Vector3.Distance(arrow.startPos, arrow.targetPos);
            float baseY = Mathf.Lerp(arrow.startPos.y, arrow.targetPos.y, progressFromStart / dist);
            float arc = (arcHeight * progressFromStart * progressToEndPos) / (-0.25f * dist * dist);
            transform.rotation = Quaternion.LookRotation(arrow.nextBasePos + new Vector3(0, baseY + arc, 0));// = LookAt2D(());
            transform.position = new Vector3(transform.position.x, baseY + arc, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        arrow.hasReachedPosition = true;
    }
}
