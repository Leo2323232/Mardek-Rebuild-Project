﻿using UnityEngine;
using System.Collections.Generic;


namespace JRPG
{
    [SelectionBase]
    public class InMapParty : MonoBehaviour
    {
        static InMapParty instance;

        [SerializeField] List<GameObject> inMapCharacters = new List<GameObject>();

        private void Awake()
        {
            if (instance)
                Destroy(instance);
            instance = this;
        }

        public static void PositionPartyAt(Vector2 position, MoveDirection facingDirection)
        {
            if (instance)
            {
                for (int i = 0; i < instance.inMapCharacters.Count; i++)
                {
                    GameObject character = instance.inMapCharacters[i];
                    if (character != null)
                    {
                        Utilities2D.SetTransformPosition(character.transform, position);
                        if (facingDirection)
                            character.GetComponent<Movement>().FaceDirection(facingDirection);
                    }
                }
            }
            else
            {
                Debug.LogError("No InMapParty found");
            }
        }
    }
}
