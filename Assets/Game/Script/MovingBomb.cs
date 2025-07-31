using UnityEngine;
namespace PhoenixaStudio
{
    public class MovingBomb : MonoBehaviour
    {
        //set the moving speed
        public float speed = 2;
        //set the moving direction
        public enum MoveDirection { Up, Down }
        public MoveDirection moveDirection;
        //only move of the distace to the player lower than this value
        public float activeDistanceToPlayer = 5;
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
                transform.Translate(new Vector3(0, speed * Time.deltaTime * (moveDirection == MoveDirection.Up ? 1 : -1), 0));
            }
        }
    }
}