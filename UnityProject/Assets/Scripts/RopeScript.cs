using UnityEngine;
using System.Collections;
using System.Linq;

// Require a Rigidbody2D and LineRenderer object for easier assembly
using System.Collections.Generic;


[RequireComponent (typeof (LineRenderer))]

public class RopeScript : MonoBehaviour {
	/*========================================
	==  Physics Based Rope				==
	==  File: Rope.js					  ==
	==  Original by: Jacob Fletcher		==
	==  Use and alter Freely			 ==
	==  CSharp Conversion by: Chelsea Hash  ==
	==========================================
	How To Use:
	 ( BASIC )
	 1. Simply add this script to the object you want a rope teathered to
	 2. In the "LineRenderer" that is added, assign a material and adjust the width settings to your likeing
	 3. Assign the other end of the rope as the "Target" object in this script
	 4. Play and enjoy!
 
	 ( Advanced )
	 1. Do as instructed above
	 2. If you want more control over the rigidbody on the ropes end go ahead and manually
		 add the rigidbody component to the target end of the rope and adjust acordingly.
	 3. adjust settings as necessary in both the rigidbody and rope script
 
	 (About Character Joints)
	 Sometimes your rope needs to be very limp and by that I mean NO SPRINGY EFFECT.
	 In order to do this, you must loosen it up using the swingAxis and twist limits.
	 For example, On my joints in my drawing app, I set the swingAxis to (0,0,1) sense
	 the only axis I want to swing is the Z axis (facing the camera) and the other settings to around -100 or 100.
 
 
	*/
	
	// List of objects to keep in focus
	public GameObject[] objects;
	
	public float ropeDrag = 0.1F;								 //  Sets each joints Drag
	public float ropeMass = 0.1F;							//  Sets each joints Mass
	public float ropeColRadius = 0.5F;	
	//public float ropeBreakForce = 25.0F;					 //-------------- TODO (Hopefully will break the rope in half...
	private Vector3[] segmentPos;			//  DONT MESS!	This is for the Line Renderer's Reference and to set up the positions of the gameObjects
	private GameObject[] joints;			//  DONT MESS!	This is the actual joint objects that will be automatically created
	private LineRenderer line;							//  DONT MESS!	 The line renderer variable is set up when its assigned as a new component
	private bool rope = false;						 //  DONT MESS!	This is to keep errors out of your debug window! Keeps the rope from rendering when it doesnt exist...
	
	public float dampingratio = 2F;
	public float frequency = 4F;
	public float springdistance = 0.000001F;
	public int segments = 20;
	
	
	[HideInInspector]
	public List<Collider2D> collidersToIgnore;
	
	[HideInInspector]
	public List<GameObject> itemsThatLoops;
	
	//Joint Settings
	
	
	void Start()
	{
		BuildRope();
	}
	
	void Reset() {
		//This lines finds the correct Collider objects in the players that the rope shall connect to
		objects = transform.parent.GetComponentsInChildren<PlayerController> ().Select (x => x.gameObject.transform.Find("Collider").gameObject).ToArray (); // wups
		//objects = GameObject.FindGameObjectsWithTag ("Player");
	}
	
	void Update()
	{
		// Put rope control here!
		
		
		//Destroy Rope Test	(Example of how you can use the rope dynamically)
		/*if(rope && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
		{
			//DestroyRope();	
		}	*/
		if(!rope && Input.GetKeyDown("r"))
		{
			BuildRope();
		}
	}
	void LateUpdate()
	{
		// Does rope exist? If so, update its position
		if(rope) {
			for(int i=0;i<segments;i++) {
				if(i == 0) {
					line.SetPosition(i,objects[0].transform.position);
				} else
				if(i == segments-1) {
					line.SetPosition(i,objects[1].transform.position);	
				} else {
					line.SetPosition(i,joints[i].transform.position);
				}
			}
			line.enabled = true;
			
		} else {
			if (objects [0] != null && objects [1] != null) {
				line.enabled = false;	
			}	
		}
	}
	
	
	
	public void BuildRope()
	{
		line = gameObject.GetComponent<LineRenderer>();
		
		line.SetVertexCount(segments);
		segmentPos = new Vector3[segments];
		joints = new GameObject[segments];
		segmentPos[0] = objects[0].transform.position;
		segmentPos[segments-1] = objects[1].transform.position;
		
		for(int s=1;s < segments;s++)
		{
			// Find the each segments position using the slope from above
			Vector3 vector = (new Vector3(springdistance, 0, 0)*s) + transform.position;	
			segmentPos[s] = vector;
			
			//Add Physics to the segments
			AddJointPhysics(s);
		}
		
		// Attach the joints to the target object and parent it to this object	
		//objects[1].transform.parent = transform;
		
		
		SpringJoint2D end = objects[1].gameObject.AddComponent<SpringJoint2D>();
		end.connectedBody = joints[joints.Length-1].transform.rigidbody2D;
		
		end.dampingRatio = dampingratio;
		end.frequency = frequency;
		end.distance = springdistance;
		end.collideConnected = false;
		
		foreach (var col in objects[0].GetComponentsInChildren<Collider2D>()){
			collidersToIgnore.Add(col);
		}
		foreach (var col in objects[1].GetComponentsInChildren<Collider2D>()){
			collidersToIgnore.Add(col);
		}
		
		itemsThatLoops.Add (objects[0]);
		itemsThatLoops.Add (objects[1]);
		collidersToIgnore.Add (objects[0].collider2D);
		collidersToIgnore.Add (objects[1].collider2D);
		collidersToIgnore.Add (end.collider2D);
		ignoreCollisionWithRope();
		
		// Rope = true, The rope now exists in the scene!
		rope = true;
	}
	
	void AddJointPhysics(int n)
	{
		joints[n] = new GameObject("Joint_" + n);
		joints[n].transform.parent = transform;
		Rigidbody2D rigid = joints[n].AddComponent<Rigidbody2D>();
		SpringJoint2D ph = joints[n].AddComponent<SpringJoint2D>();
		//DistanceJoint2D ph = joints[n].AddComponent<DistanceJoint2D>();
		CircleCollider2D col = joints[n].AddComponent<CircleCollider2D>();
		joints [n].AddComponent<RopeHitScript> ();
		col.enabled = true;
		col.isTrigger = false;
		col.radius = ropeColRadius;
		
		//ph.breakForce = ropeBreakForce; <--------------- TODO
		ph.dampingRatio = dampingratio;
		ph.frequency = frequency;
		ph.collideConnected = false;
		ph.distance = springdistance;
		
		//Add collider
		//TODO
		
		
		
		
		joints[n].transform.position = segmentPos[n];
		
		rigid.drag = ropeDrag;
		rigid.mass = ropeMass;
		//rigid.isKinematic = true;

		itemsThatLoops.Add(joints [n]);
		collidersToIgnore.Add (col);
		
		if(n==1){		
			ph.connectedBody = objects[0].transform.rigidbody2D;
		} else
		{
			ph.connectedBody = joints[n-1].rigidbody2D;	
		}
		
	}
	
	void ignoreCollisionWithRope() {
		
		foreach (var item in itemsThatLoops) {
			if (item){
				Collider2D col = item.collider2D;
				foreach (Collider2D c in collidersToIgnore) {
				Physics2D.IgnoreCollision (col, c);
				}
			}
		}
	}
	
	public void DestroyRope()
	{
		// Stop Rendering Rope then Destroy all of its components
		rope = false;
		for(int dj=0;dj<joints.Length-1;dj++)
		{
			Destroy(joints[dj]);
			Destroy(objects[1].GetComponent<SpringJoint2D>());
		}
		
		//segmentPos = new Vector3[0];
		//joints = new GameObject[0];
		//segments = 0;
	}

	/// <summary>
	/// Resets the position of the rope.
	/// Spaces out the segments so that they are evenly spaced between the two connecting objects.
	/// Useful when moving the objects around (e.g. respawns), so that the rope doesn't freak out.
	/// </summary>
	public void ResetPosition() {
		Vector2 pos1 = objects[0].transform.position;
		Vector2 pos2 = objects[1].transform.position;
		
		// For some very odd reason, joints[0] is never set. we start past that, for now
		for (int i = 1; i < joints.Length; i++) {
			float t = i / joints.Length;
			joints[i].transform.position = Vector2.Lerp(pos1, pos2, t);
			joints[i].rigidbody2D.velocity = Vector2.zero;
			joints[i].rigidbody2D.angularVelocity = 0.0f;
		}
	}
	
}