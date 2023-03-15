using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    [ProtoContract]
    public class TransformState : IProtoMessage
    {
        [ProtoMember(1)]
        public float posX;
        [ProtoMember(2)]
        public float posY;
        [ProtoMember(3)]
        public float posZ;

        [ProtoMember(4)]
        public float rotX;
        [ProtoMember(5)]
        public float rotY;
        [ProtoMember(6)]
        public float rotZ;
        [ProtoMember(7)]
        public float rotW;

        internal ulong seqNo = 0;
        public TransformState() { } 
        internal TransformState(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation) 
        {
            posX = position.x;
            posY = position.y;
            posZ = position.z;
            rotX = rotation.x;
            rotY = rotation.y;
            rotZ = rotation.z;
            rotW = rotation.w;
        }
    }
    [ProtoContract]
    public class ProtoVector2
    {
        [ProtoMember(1)]
        public float x;

        [ProtoMember(2)]
        public float y;

        public ProtoVector2() { }
        public ProtoVector2(UnityEngine.Vector2 v)
        {
            x = v.x;
            y = v.y;
        }
    }

    [ProtoContract]
    public class PlayerState : IProtoMessage
    {
        [ProtoMember(1)]
        public TransformState playerCoordinates;

        [ProtoMember(2)]
        public Dictionary<Guid, TransformState> bulletStates;

        [ProtoMember(3)]
        public Dictionary<Guid, MissileState> missileStates;

        [ProtoMember(4)]
        public DateTime TimeStamp;

        [ProtoMember(5)]
        public ulong sequenceNumber;

        [ProtoMember(6)]
        public ProtoVector2 UserInputKeyboardDirection;
        [ProtoMember(7)]
        public ProtoVector2 UserInputMouseDirection;
        [ProtoMember(8)]
        public bool UserInputBoosterOn;

        public Guid PlayerId;
    }


    [ProtoContract]
    public class MissileState : IProtoMessage
    {
        [ProtoMember(1)]
        public TransformState missileCoordinates;

        [ProtoMember(2)]
        public TransformState targetCoordinates;
        [ProtoMember(3)]
        public string targetName;
        [ProtoMember(4)]
        public Guid MissileId;

        [ProtoMember(5)]
        public Guid TargetId;
    }


}
