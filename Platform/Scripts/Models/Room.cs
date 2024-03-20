/*******************************************************************************
Copyright © 2015-2022 PICO Technology Co., Ltd.All rights reserved.

NOTICE：All information contained herein is, and remains the property of
PICO Technology Co., Ltd. The intellectual and technical concepts
contained herein are proprietary to PICO Technology Co., Ltd. and may be
covered by patents, patents in process, and are protected by trade secret or
copyright law. Dissemination of this information or reproduction of this
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd.
*******************************************************************************/

using System;
using System.Collections.Generic;

namespace Pico.Platform.Models
{

    /// <summary>
    /// Room info.
    /// </summary>
    public class Room
    {
        /// The datastore that stores a room's metadata. The maximum datastore key length is 32 bytes and the maximum datastore value length is 64 bytes. 
        public readonly Dictionary<string, string> DataStore;
        /// Room description. The maximum length is 128 bytes. 
        public readonly string Description;
        /// Room ID. 
        public readonly UInt64 RoomId;
        /// Whether the room is locked. 
        public readonly bool IsMembershipLocked;
        /// Room's join policy. 
        public readonly RoomJoinPolicy RoomJoinPolicy;
        /// Room's joinability. 
        public readonly RoomJoinability RoomJoinability;
        /// The maximum number of users allowed to join a room, which is `100`. 
        public readonly uint MaxUsers;
        /// Room owner. This field can be null. Need to check whether it is null before use. 
        public readonly User OwnerOptional;
        /// Room type. 
        public readonly RoomType RoomType;
        /// Room members. This field can be null. Need to check whether it is null before use. 
        public readonly UserList UsersOptional;
        /// Room name. 
        public readonly string Name;
        /// The Num of the users in room.
        public readonly uint PlayerNumber;

        public Room(IntPtr o)
        {
            PlayerNumber = CLIB.ppf_Room_GetPlayerNumber(o);
            DataStore = CLIB.DataStoreFromNative(CLIB.ppf_Room_GetDataStore(o));
            Description = CLIB.ppf_Room_GetDescription(o);
            RoomId = CLIB.ppf_Room_GetID(o);
            IsMembershipLocked = CLIB.ppf_Room_GetIsMembershipLocked(o);
            RoomJoinPolicy = (RoomJoinPolicy) CLIB.ppf_Room_GetJoinPolicy(o);
            RoomJoinability = (RoomJoinability) CLIB.ppf_Room_GetJoinability(o);
            MaxUsers = CLIB.ppf_Room_GetMaxUsers(o);
            Name = CLIB.ppf_Room_GetName(o);
            RoomType = (RoomType) CLIB.ppf_Room_GetType(o);
            {
                var ptr = CLIB.ppf_Room_GetOwner(o);
                if (ptr == IntPtr.Zero)
                {
                    OwnerOptional = null;
                }
                else
                {
                    OwnerOptional = new User(ptr);
                }
            }

            {
                var ptr = CLIB.ppf_Room_GetUsers(o);
                if (ptr == IntPtr.Zero)
                {
                    UsersOptional = null;
                }
                else
                {
                    UsersOptional = new UserList(ptr);
                }
            }
        }
    }

    /// <summary>Room list info. Each element is \ref Room.</summary>
    public class RoomList : MessageArray<Room>
    {
        /// The total number of rooms. 
        public readonly int TotalCount;
        /// The current page idex from which the list begins. 
        public int CurIndex;
        /// The number of rooms given on each page. 
        public int PageSize;

        public RoomList(IntPtr a)
        {
            TotalCount = CLIB.ppf_RoomArray_GetTotalCount(a);
            CurIndex = CLIB.ppf_RoomArray_GetPageIndex(a);
            PageSize = CLIB.ppf_RoomArray_GetPageSize(a);
            NextPageParam = CLIB.ppf_RoomArray_HasNextPage(a) ? "true" : string.Empty;
            int count = (int) CLIB.ppf_RoomArray_GetSize(a);
            this.Capacity = count;
            for (uint i = 0; i < count; i++)
            {
                this.Add(new Room(CLIB.ppf_RoomArray_GetElement(a, (UIntPtr)i)));
            }
        }
    }
}