using UnityEngine;
namespace PhoenixaStudio
{
    public class MovingBomb : MonoBehaviour
    {
        //set the moving speed
        public float speed = 2;
        //set the moving direction
        public enum MoveDirection { Up, Down, left, right }
        public MoveDirection moveDirection;
        //only move of the distace to the player lower than this value
        public float activeDistanceToPlayer = 5;
        public Transform iceBall;
        bool isWorking = false;

        void Update()
        {
            //Waiting and check the distace to player
            if (!isWorking)
            {
                if (Mathf.Abs(GameManager.Instance.Player.transform.position.x - transform.position.x) < activeDistanceToPlayer)
                {
                    //allow moving
                    isWorking = true;
                }
            }
            else
            {
                switch (moveDirection)
                {
                    case MoveDirection.Up:
                        transform.Translate(new Vector3(0, speed * Time.deltaTime * 1), 0);
                        break;
                    case MoveDirection.Down:
                        transform.Translate(new Vector3(0, speed * Time.deltaTime * -1), 0);
                        break;
                    case MoveDirection.left:
                        transform.Translate(new Vector3(speed * Time.deltaTime *  -1 , 0), 0);
                        if(iceBall) iceBall.Rotate(0f, 0f, speed*20 * Time.deltaTime);
                        break;


                }
                //transform.Translate(new Vector3(0, speed * Time.deltaTime * (moveDirection == MoveDirection.Up ? 1 : -1), 0));
            }
        }
    }
}