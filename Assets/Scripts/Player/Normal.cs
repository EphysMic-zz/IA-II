using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal : Bullets{
    //La bala rapida funciona desde este script, pero en Slimehero se cambia la velocidad :V

    public Vector3 mouseRay;
    Camera cam;

    public void Update() {
        this.transform.position += this.transform.forward * Time.deltaTime * speed;
    }

    public override void DispenseBullets() {
        base.DispenseBullets();
        mouseRay = GetRay();
        Vector3 dir = new Vector3(mouseRay.x, discharger.transform.position.y, mouseRay.z) - discharger.transform.position;
        this.transform.forward = dir;
        this.transform.position = discharger.transform.position;
        Instantiate(this.gameObject); // BAM
        //Debug
        Debug.DrawRay(discharger.transform.position, dir, Color.white, 10, false);//BLANCO
    }

    public Vector3 GetRay() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if ( Physics.Raycast(ray, out hit) ) {
            return hit.point;
        }
        else
            return Vector3.zero;
    }
}
