using Unity.Entities;
using UnityEngine;

namespace VoxelGame.Player
{
    public partial class PlayerInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            int moveX = 0;
            int moveZ = 0;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveZ = 1;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveZ = -1;
            
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX = 1;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX = -1;

            bool isCasting = Input.GetKey(KeyCode.Space);

            foreach (var input in SystemAPI.Query<RefRW<PlayerInputComponent>>())
            {
                input.ValueRW.MoveX = moveX;
                input.ValueRW.MoveZ = moveZ;
                input.ValueRW.IsCasting = isCasting;
            }
        }
    }
}
