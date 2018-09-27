using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tux : MonoBehaviour {
    public enum MOVEDIR { UP, DOWN, LEFT, RIGHT}
    private class Point
    {
        private int XPos { get; set; }
        private int Ypos { get; set; }
        private Stack historyStack;
        public Point(int x, int y)
        {
            XPos = x;
            Ypos = y;
        }

    }
    [SerializeField]
    private float speed;
    private bool idle, moving;
    private int moveCooldown;
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
    private Animator animator;
    private bool flipAnimator;
    [SerializeField]
    private Tilemap[] tilemap;




    // Use this for initialization
    void Start ()
    {
        speed = 2.0F;
        dir = MOVEDIR.DOWN;
        moveCooldown = 0;
        idle = true; moving = false;
        moveDivisor = 9;
        gridPosition = new Vector3(0, 3, 0);
        gameboard = FindObjectOfType<Gameboard>();
        if (null == gameboard)
        {
            gameboard = ScriptableObject.CreateInstance<Gameboard>();
            gameboard.Start();
        }
        gridPosition =  gameboard.getStartVector();
        animator = GetComponent<Animator>();
        flipAnimator = false;


    }


    // Update is called once per frame
    void Update () {

        moveCooldown--;
        mySpace = gameboard.GetSpace((int)gridPosition.x, (int)gridPosition.y);
        if (idle)
        {
            position = transform.position;
            gridMove();
        }
        if (moving)
        {
            if (transform.position == position)
            {
                moving = false;
                idle = true;
                gridMove();
            }
            transform.position = Vector2.MoveTowards(transform.position, position, Time.deltaTime * speed);
        }
        if (mySpace == "door")
        {
            moving = false;
            Door openDoor = gameboard.GetDoor(1, 1);
            transform.position = new Vector2(openDoor.getDestinationMapx(), openDoor.getDestinationMapy());
            gridPosition.x = openDoor.getDestinationGridX();
            gridPosition.y = openDoor.getDestinationGridY();
            moveCooldown = 0;
            position = transform.position;
            gameboard.UseDoor(1, 1);
            idle = true;
        }

	}
    // Move Tux Grid-aligned
    private void gridMove()
    {
        if (moveCooldown <= 0)
        {
            if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow)) {
                if (dir != MOVEDIR.UP) { dir = MOVEDIR.UP; moveCooldown = 5;
                    animator.Play("tux-up"); }
                else
                {
                    
                    if (CanMove(gridPosition.x, gridPosition.y, gridPosition.x, gridPosition.y + 1))
                    {
                        idle = false;
                        moving = true;
                        position += (Vector3) Vector3.up / moveDivisor;
                        gridPosition += Vector3.up;
                 
                    }                
                }
            }
            else if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow))
            {
                if (dir != MOVEDIR.LEFT) {
                    dir = MOVEDIR.LEFT; moveCooldown = 5;
                    if (flipAnimator == false)
                    {
                        animator.transform.Rotate(0, 180, 0);
                        flipAnimator = true;
                    }
                    animator.Play("tux-left");
                }
                else
                {
                    if (CanMove(gridPosition.x, gridPosition.y, gridPosition.x-1, gridPosition.y))
                    {
                        idle = false;
                        moving = true;
                        position += Vector3.left / moveDivisor;
                        gridPosition += Vector3.left;
                    }
                }
            }
            else if (Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow))
            {
                if (dir != MOVEDIR.DOWN) { dir = MOVEDIR.DOWN; moveCooldown = 5; animator.Play("tux-idle"); }
                else
                {
                    if (CanMove(gridPosition.x, gridPosition.y, gridPosition.x , gridPosition.y -1))
                    {
                        idle = false;
                        moving = true;
                        position += Vector3.down / moveDivisor;
                        gridPosition += Vector3.down;
                    }
                        
                }
            }
            else if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow))
            {
                if (dir != MOVEDIR.RIGHT) { dir = MOVEDIR.RIGHT; moveCooldown = 5;
                    if (flipAnimator == true)
                    {
                        animator.transform.Rotate(0, 180, 0);
                        flipAnimator = false;
                    }
                    animator.Play("tux-right");
                }
                else
                {
                    if (CanMove(gridPosition.x, gridPosition.y, gridPosition.x+1, gridPosition.y))
                    {
                        idle = false;
                        moving = true;
                        position += Vector3.right / moveDivisor;
                        gridPosition += Vector3.right;
                    }
                        
                }
            }
        }
      /*  if (gameboard.GetSpace((int) gridPosition.x, (int) gridPosition.y).Equals("door"))
      //  {
            Debug.Log("Calling door");
        }
    */
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
 
       // Debug.Log("Spaces("+thisSpace+","+destSpace+")");
        if (destSpace == "")
        {
            return false;
        }
        else if (thisSpace==destSpace)
        {
            return true;
        }
      else if (destSpace == null)
        {
            return true;
        }
      else if (("stair".Contains(thisSpace)) && ( "grass,path".Contains(destSpace)))
        { return true; }
      else if ("stair,TUX".Contains(destSpace))
        {
         //   Debug.Log("" + gameboard.GetSpace(_dx, _dy));

            return true;
        }
        else if ("door".Contains(destSpace))
        {
           // Debug.Log("" + gameboard.GetSpace(_dx, _dy));
          //  Door temp = gameboard.UseDoor(_dx, _dy);

            return true;
        }
        else if ((thisSpace=="TUX") && ("grass,path,stair".Contains(destSpace)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("There was a collission!");
    }

    public void DamageCollision(string npc, float dmg)
    {
        Debug.Log("You've just collided with "+npc+" it did " + dmg + " damage.");
        return;
    }

    // read the current tilemap

}
