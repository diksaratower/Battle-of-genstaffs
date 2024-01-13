using System.Collections;
using UnityEngine;


public class DivisionTemplateModelPreviewModelGO : MonoBehaviour
{
    private const float speed = 0.014f;

    private void Start()
    {
        StartCoroutine(RotateDivisionPreviewModel());
    }

    private IEnumerator RotateDivisionPreviewModel()
    {
        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(speed);

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
                transform.localEulerAngles.y + 1,
                transform.localEulerAngles.z);

            yield return new WaitForSeconds(speed);
        }
    }
}
