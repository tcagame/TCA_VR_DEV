using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {

    [SerializeField]
    GameObject particle;
    [SerializeField]
    GameObject particle_Departure;

    private GameObject Particle_prefab_trajectory;
    private GameObject Particle_prefab_Hit;
    private GameObject Particle_trajectory;
    private GameObject Particle_hit;

    public LayerMask mask;

    //ParticleSystemコントローラーステート
    public enum PARTICLE_CONTROLLER
    {
        PARTICLE_PLAY,
        PARTICLE_STOP,
        PARTICLE_PAUSE
    };

    //ParticleSystem軌跡
    public enum PARTICLE_TAG
    {
        PARTICLE_TRAJECTORY,
        PARTICLE_A
    };

    void Start () {
        Particle_prefab_trajectory = (GameObject)Resources.Load("Particles/particle_Departure");
        Particle_prefab_Hit = (GameObject)Resources.Load("Particles/particle_Hit");
    }

    //ParticleSystemコントローラー
    public void particleAction( PARTICLE_CONTROLLER tag ) {
        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool Ishit = Physics.Raycast(raycast, out hit, 0.75f, mask.value);
        Ishit = true;
        switch ( tag ) {
            case PARTICLE_CONTROLLER.PARTICLE_PLAY:
                if (Ishit == true)
                {
                    particle.transform.position = raycast.origin + hit.distance * raycast.direction.normalized;
                    //particle.Play();
                }
                break;
            case PARTICLE_CONTROLLER.PARTICLE_STOP:
                if (Ishit == true)
                {
                    particle.transform.position = raycast.origin + hit.distance * raycast.direction.normalized;
                   // particle.Stop();
                }
                break;
            case PARTICLE_CONTROLLER.PARTICLE_PAUSE:
                if (Ishit == true)
                {
                    particle.transform.position = raycast.origin + hit.distance * raycast.direction.normalized;
                   // particle.Pause();
                }
                break;
        }
       
    }

    //ParticleSystem軌跡を描画
    public void particleDeparture(PARTICLE_TAG tag, Transform tr) {
        bool Departure = true;
        switch(tag){
        case PARTICLE_TAG.PARTICLE_TRAJECTORY:
            if(Departure == true)
            {
                Vector3 vecHeartPos = particle_Departure.gameObject.transform.position;
                Particle_trajectory = (GameObject)Instantiate(Particle_prefab_trajectory, vecHeartPos, Quaternion.identity);
                Particle_trajectory.transform.parent = particle_Departure.gameObject.transform;
            }
            break;
            case PARTICLE_TAG.PARTICLE_A:
            if (Departure == true)
            {
                    //                Vector3 vecHeartPos = particle.gameObject.transform.position;
                    //                Particle_hit = (GameObject)Instantiate(Particle_prefab_Hit, vecHeartPos, Quaternion.identity);
                    Particle_hit = (GameObject)Instantiate(Particle_prefab_Hit, tr.position, tr.rotation);
                Destroy(Particle_hit, 1.0f);
            }
                break;
        }
    }
}
