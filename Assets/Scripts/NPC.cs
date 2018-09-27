using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
    [SerializeField]
    public enum MOVEDIR { UP, DOWN, LEFT, RIGHT }
    [SerializeField]
    private float speed;
    private bool idle, moving;
    private int moveCooldown;
    [SerializeField]
    private MOVEDIR dir;
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Vector3 gridPosition;
    [SerializeField]
    private int moveDivisor;
    private Gameboard gameboard;
    [SerializeField]
    private string mySpace;
    [SerializeField]
    private int modulo;
    [SerializeField]
    private int homeLevel;
    private Animator animator;
    private bool flipAnimator;
    private Tux PC;




    // Use this for initialization
    void Start()
    {
     modulo = 10;
     speed = 1F;
   //     null == dir : dir = MOVEDIR.DOWN ? ; 
     moveCooldown = 0;
     idle = false; moving = false;
     moveDivisor = 9;
     gameboard = FindObjectOfType<Gameboard>();
     if (null == gameboard)
        {
            gameboard = ScriptableObject.CreateInstance<Gameboard>();
            gameboard.Start();
        }
    
     PC = GetComponent<Tux>(); 
      
    // gridPosition = new Vector3(9F, 3F, 0F);
     animator = GetComponent<Animator>();
     flipAnimator = false;
       // animator.Play("tux-up");
    }

    // Update is called once per frame
    void Update()
    {

        //  moveCooldown//++;
        // mySpace = gameboard.GetSpace((int)gridPosition.x, (int)gridPosition.y);
        if (gameboard.GetIndex() == homeLevel)
        {
           // CheckCollision(PC);
            if (!moving)
            {
                gridMove();
                moving = true;
            }
            if (moving)
            {
                transform.position = Vector2.MoveTowards(transform.position, position, Time.deltaTime * speed);
                if (Vector2.Distance(transform.position, position) < 0.01F)
                {
                    moving = false;
                }
            }
        }
    }
    // Move Tux Grid-aligned
    private void gridMove()
    {              
        if (dir == MOVEDIR.UP)
        {
            if (CanMove(gridPosition.x, gridPosition.y, gridPosition.x, gridPosition.y + 1))
                {
                                        
                    position += (Vector3)Vector3.up / moveDivisor;
                    gridPosition += Vector3.up;
                }
            else
            { dir = MOVEDIR.DOWN; }

        }
        else if (dir == MOVEDIR.LEFT)
        {              
            if (CanMove(gridPosition.x, gridPosition.y, gridPosition.x - 1, gridPosition.y))
            {
                position += Vector3.left / moveDivisor;
                gridPosition += Vector3.left;
            }
            else
             { dir = MOVEDIR.RIGHT; }

        }
        else if (dir == MOVEDIR.DOWN)
        {
            if (CanMove(gridPosition.x, gridPosition.y, gridPosition.x, gridPosition.y - 1))
            {
                position += Vector3.down / moveDivisor;
                gridPosition += Vector3.down;
            }
            else
            { dir = MOVEDIR.UP;}
        }
        else if (dir == MOVEDIR.RIGHT)
        {               
            if (CanMove(gridPosition.x, gridPosition.y, gridPosition.x + 1, gridPosition.y))
                {
                    position += Vector3.right / moveDivisor;
                    gridPosition += Vector3.right;
                }
            else
            {dir = MOVEDIR.LEFT;}

        }
       
    }
    // Move Tux
    private bool CanMove(float x, float y, float destX, float destY)
    {
        //return true;
        int _x = (int)x;
        int _y = (int)y;
        int _dx = (int)destX;
        int _dy = (int)destY;
        string thisSpace = gameboard.GetSpace(_x, _y);
        string destSpace = gameboard.GetSpace(_dx, _dy);

        //Debug.Log("Spaces(" + thisSpace + "," + destSpace + ")");
        if (destSpace == "")
        {
            return false;
        }
        else if (thisSpace == destSpace)
        {
            return true;
        }
        else if (destSpace == null)
        {
            return true;
        }
        else if (("stair".Contains(thisSpace)) && ("grass,path".Contains(destSpace)))
        { return true; }
        else if ("stair,TUX".Contains(destSpace))
        {
          //  Debug.Log("" + gameboard.GetSpace(_dx, _dy));

            return true;
        }
        else if ("door".Contains(destSpace))
        {
           // Debug.Log("" + gameboard.GetSpace(_dx, _dy));
            //  Door temp = gameboard.UseDoor(_dx, _dy);

            return true;
        }
        else if ((thisSpace == "TUX") && ("grass,path,stair".Contains(destSpace)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // See if they have collided with something

    private void CheckCollision(Tux thing)
    {
        if (Vector3.Distance(thing.transform.position, this.transform.position) < 0.1)
        {
            thing.DamageCollision("NPC", 1f);
        }
    }

}
