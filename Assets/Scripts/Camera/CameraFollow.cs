using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform Target;
    public Vector3 OffSet;

    public float SmoothTime = 0.05f;
    public Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    private void LateUpdate()
    {
        if (Target != null)
        {
            //Actualizamos nuestra posición
            Vector3 rotatedPosition = Target.position + ((-Target.forward * OffSet.z) + (Target.up * OffSet.y));
            transform.position = Vector3.SmoothDamp(transform.position, rotatedPosition, ref velocity, SmoothTime);

            //Actualizamos la dirección a la que queremos ver.
            Vector3 LookToTarget = (Target.position - transform.position).normalized;
            transform.forward = LookToTarget;

            /* -----------> NOTAS <-------------- 
             * Si el cambio de posición está suavizado, la actualización del forward también se suavizará por defecto.
             * Para calcular la posición global, debemos sumar a tu posición, tu vector forward.
            */
        }
    }

    public void SetTarget(GameObject Target)
    {
        this.Target = Target.transform;
    }
}
