using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class GazeDuration : MonoBehaviour
{
    /* RayCasting Parameters */
    public float sphereCastRadius;      // set the radius for the sphere when using SphereCasting
    public float maxDistance;           // reach threshold for the casting
    public LayerMask mask;              // only the object specified by the mask (in inspector) will be detected by the RayCasting.
    private Vector3 origin;             // the start position of the beam
    private Vector3 direction;          // direction of the beam

    /* Miscellanous Stuff */
    public GameObject target;
    private GameObject player;
    private GameObject latestHitObject;
    private bool onObjectGaze = false;
    private Ray ray;
    private Dictionary<string, float> timerHashTable, gazeDurationTable;

    void Start()
    {
        player = GameObject.Find("PlayerCamera");
        ray = new Ray();
        timerHashTable = new Dictionary<string, float>();
        gazeDurationTable = new Dictionary<string, float>();
    }

    /* Update is called once per frame */
    void Update()
    {
        updateRayCastVector();
        updateMaxDistance();

        rayCast();                    // tiny laser beams
        //sphereCast();               // wider laser beams
        saveData();
    }

    private void updateRayCastVector()
    {
        origin = transform.position;
        direction = transform.forward;
    }

    private void updateMaxDistance()
    {
        maxDistance = Vector3.Distance(player.transform.position, target.transform.position);
    }

    private void rayCast()
    {
        ray.direction = direction;
        ray.origin = origin;

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 10, mask))
        {
            OnObjectHit(hitInfo);
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
        else
        {
            OnObjectNotHit();
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100);
        }
    }

    private void OnObjectHit(RaycastHit hitInfo)
    {
        GameObject currentHitObject = hitInfo.transform.gameObject;

        if (!onObjectGaze)
        {
            onObjectGaze = true;

            if (!timerHashTable.ContainsKey(currentHitObject.tag))
            {
                timerHashTable.Add(currentHitObject.tag, 0f);
            }
            timerHashTable[currentHitObject.tag] = Time.time;
        }

        latestHitObject = currentHitObject;
    }

    private void OnObjectNotHit()
    {
        if (onObjectGaze)
        {
            onObjectGaze = false;

            if (!gazeDurationTable.ContainsKey(latestHitObject.tag))
            {
                gazeDurationTable.Add(latestHitObject.tag, 0f);
            }

            gazeDurationTable[latestHitObject.tag] += Time.time - timerHashTable[latestHitObject.tag];

            Debug.Log("object: " + latestHitObject.tag + " time:" + gazeDurationTable[latestHitObject.tag]);
        }
    }


    private void sphereCast()
    {
        RaycastHit hitInfo;

        if (Physics.SphereCast(origin, sphereCastRadius, direction, out hitInfo, maxDistance, mask))
            OnObjectHit(hitInfo);
        else
            OnObjectNotHit();
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(origin, origin + direction * currentHitDistance);
        Gizmos.DrawWireSphere(origin + direction * currentHitDistance, castRadius);
    }
    */

    private XmlWriterSettings _fileSettings;
    private XmlWriter _file;
    [SerializeField]
    [Tooltip("Folder in the application root directory where data is saved.")]
    private string _folder = "Data";

    private void saveData()
    {

        if (_file == null)
        {
            // Opens data file. It becomes non-null.
            OpenDataFile();
        }
    }

    private void OpenDataFile()
    {
        if (_file != null)
        {
            Debug.Log("Already saving data.");
            return;
        }

        if (!System.IO.Directory.Exists(_folder))
        {
            System.IO.Directory.CreateDirectory(_folder);
        }

        _fileSettings = new XmlWriterSettings();
        _fileSettings.Indent = true;
        var fileName = string.Format("vr_data_{0}.xml", System.DateTime.Now.ToString("yyyyMMddTHHmmss"));
        _file = XmlWriter.Create(System.IO.Path.Combine(_folder, fileName), _fileSettings);
        _file.WriteStartDocument();
        _file.WriteStartElement("Data");
    }

    private void OnDestroy()
    {
        _file.WriteStartElement("GazeDurationData");
        foreach (KeyValuePair<string, float> entry in gazeDurationTable)
        {
            _file.WriteAttributeString(entry.Key, entry.Value.ToString());
        }
        
        /*_file.HMDWritePose(gazeData.Pose);
        _file.HMDWriteEye(gazeData.Left, "Left");
        _file.HMDWriteEye(gazeData.Right, "Right");
        _file.WriteRay(gazeData.CombinedGazeRayWorld, gazeData.CombinedGazeRayWorldValid, "CombinedGazeRayWorld");*/
        _file.WriteEndElement();
        CloseDataFile();
    }


    private void CloseDataFile()
    {
        if (_file == null)
        {
            return;
        }

        _file.WriteEndElement();
        _file.WriteEndDocument();
        _file.Flush();
        _file.Close();
        _file = null;
        _fileSettings = null;
    }


}
