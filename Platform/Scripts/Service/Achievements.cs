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
using System.Runtime.InteropServices;
using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     *
     * The Achievements service can help build a "positive feedback mechanism"
     * in your games. You can create prizes such as trophies and badges and
     * distribute them to players when they hit a goal, like completing the
     * beginner tutorial or reaching level x. Advanced achievements such as
     * completing a hidden level/task should be closely integrated with game
     * content design and, meanwhile, collaborate with prizes like diamonds
     * or props to make your games more challenging and further enhance players'
     * engagement.
     */
    public static class AchievementsService
    {
        /// <summary>Adds a count to a specified count achievement. The count will be added to the current count. For example,
        /// if the current count is 1 and the count you would like to add is 7, the final count will be 8 if the request succeeds.
        /// @note Available to count achievements only. 
        /// </summary>
        /// <param name="name">The API name of the achievement.</param>
        /// <param name="count">The count you want to add. The largest count supported by this function is the maximum
        /// value of a signed 64-bit integer. If the count is larger than that, it is
        /// clamped to that maximum value before being passed to the servers.
        /// </param>
        /// <param name="extraData">Custom extension fields that can be used to record key information when unlocking achievements.</param>
        /// <returns>The request ID of this async function.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10729|invalid api name|
        /// |10733|invalid count|
        /// |10725|extra data too long|
        /// |10720|achievement is not exist|
        /// |10723|load achievement data failed|
        /// |10726|achievement is unreleased|
        /// |10727|achievement is archived|
        /// |10722|no write permission|
        /// |10736|invalid parameter|
        /// |10735|invalid extra data|
        /// |10734|operation is not allowed on the type|
        /// |10728|achievement is unlocked|
        /// |10724|save achievement data failed|
        ///
        /// A message of type `MessageType.Achievements_AddCount` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `AchievementUpdate`.
        /// Extract the payload from the message handle with `Message.Data`.
        ///
        /// `AchievementUpdate` contains the following:
        /// * `JustUnlocked`: Whether the achievement has been successfully unlocked.
        /// * `Name`: The API name of the achievement.
        /// </returns>
        public static Task<AchievementUpdate> AddCount(string name, long count, byte[] extraData)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            GCHandle hobj = GCHandle.Alloc(extraData, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var result = new Task<AchievementUpdate>(CLIB.ppf_Achievements_AddCount(name, count, pobj, (uint) (extraData != null ? extraData.Length : 0)));
            if (hobj.IsAllocated)
                hobj.Free();
            return result;
        }

        /// <summary>Unlocks the bit(s) of a specified bitfield achievement. The status of the bit(s) is then unchangeable.
        /// @note Available to bitfield achievements only.
        /// </summary>
        /// <param name="name">The API name of the achievement to unlock bit(s) for.</param>
        /// <param name="fields">A string containing either the `0` or `1` characters, for example, `100011`. Every `1` will unlock a bit in the corresponding position of a bitfield.</param>
        /// <param name="extraData">Custom extension fields that can be used to record key information when unlocking achievements.</param>
        /// <returns>The request ID of this async function.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10729|invalid api name|
        /// |10731|invalid field|
        /// |10725|extra data too long|
        /// |10720|achievement is not exist|
        /// |10723|load achievement data failed|
        /// |10726|achievement is unreleased|
        /// |10727|achievement is archived|
        /// |10722|no write permission|
        /// |10736|invalid parameter|
        /// |10735|invalid extra data|
        /// |10734|operation is not allowed on the type|
        /// |10728|achievement is unlocked|
        /// |10724|save achievement data failed|
        ///
        /// A message of type `MessageType.Achievements_AddFields` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `AchievementUpdate`.
        /// Extract the payload from the message handle with `Message.Data`.
        ///
        /// `AchievementUpdate` contains the following:
        /// * `JustUnlocked`: Whether the achievement has been successfully unlocked.
        /// * `Name`: The API name of the achievement.
        /// </returns>
        public static Task<AchievementUpdate> AddFields(string name, string fields, byte[] extraData)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            GCHandle hobj = GCHandle.Alloc(extraData, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var result = new Task<AchievementUpdate>(CLIB.ppf_Achievements_AddFields(name, fields, pobj, (uint) (extraData != null ? extraData.Length : 0)));
            if (hobj.IsAllocated)
                hobj.Free();
            return result;
        }

        /// <summary>Gets the information about all achievements, including API names, descriptions, types,
        /// the targets which must be reached to unlock those achievements, and more.</summary>
        /// <param name="pageIdx">Defines which page of achievements to return. The first page index is `0`.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>The request ID of this async function.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10721|invalid api name|
        /// |10736|invalid parameter|
        /// |10720|achievement is not exist|
        /// |10723|load achievement data failed|
        ///
        /// A message of type `MessageType.Achievements_GetAllDefinitions` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `AchievementDefinitionList`.
        /// Extract the payload from the message handle with `Message.Data`.
        ///
        /// `AchievementDefinitionList` contains the following:
        /// * `Type`: The type of the achievement.
        /// * `Name`: The API name of the achievement.
        /// * `BitfieldLength`: The total bits in the bitfield. For bitfield achievements only.
        /// * `Target`: The number of events to complete for unlocking the achievement. For count or bitfield achievements only.
        /// * `Description`: The description of the achievement.
        /// * `Title`: The display name of the achievement that users see.
        /// * `IsArchived`: Whether the achievement is archived. Archiving will not delete the achievement or users' progress on it.
        /// * `IsSecret`: Whether the achievement is hidden until it is unlocked by users.
        /// * `ID`: The data ID.
        /// * `UnlockedDescription`: The message displayed to users when they unlock the achievement.
        /// * `WritePolicy`: Who are able to write achievement progress.
        /// * `LockedImageURL`: The local path to the image displayed to users before they unlock the achievement.
        /// * `UnlockedImageURL`: The local path to the image displayed to users after they unlock the achievement.
        /// </returns>
        public static Task<AchievementDefinitionList> GetAllDefinitions(int pageIdx, int pageSize)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            return new Task<AchievementDefinitionList>(CLIB.ppf_Achievements_GetAllDefinitions(pageIdx, pageSize));
        }

        /// <summary>Gets the user's progress on all achievements, including API names,
        /// whether or not the achievements are unlocked, the time at which they were unlocked,
        /// achievement types and, depending on the type, the progress made towards unlocking them, and more.</summary>
        /// <param name="pageIdx">Defines which page of achievements to return. The first page index is `0`.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>The request ID of this async function.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10721|invalid api name|
        /// |10723|load achievement data failed|
        ///
        /// A message of type `MessageType.Achievements_GetAllProgress` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `AchievementProgressList`.
        /// Extract the payload from the message handle with `Message.Data`.
        ///
        /// `AchievementProgressList` contains the following:
        /// * `ID`: The data ID.
        /// * `Bitfield`: A bitfield displaying the bits unlocked for a bitfield achievement, for example, `1110001`.
        /// * `Count`: The number of events completed for unlocking a count achievement.
        /// * `IsUnlocked`: Whether the achievement is unlocked.
        /// * `Name`: The API name of the achievement.
        /// * `UnlockTime`: The time at which the achievement was unlocked.
        /// * `ExtraData`: The key information recorded when unlocking the achievement.
        /// </returns>
        public static Task<AchievementProgressList> GetAllProgress(int pageIdx, int pageSize)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            return new Task<AchievementProgressList>(CLIB.ppf_Achievements_GetAllProgress(pageIdx, pageSize));
        }

        /// <summary>Gets the information about specified achievements, including API names, descriptions, types,
        /// the targets which must be reached to unlock those achievements, and more.</summary>
        /// <param name="names">The API names of the achievements.</param>
        /// <returns>The request ID of this async function.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10729|invalid api name|
        /// |10730|too many api names|
        /// |10721|invalid request|
        /// |10736|invalid parameter|
        /// |10720|achievement is not exist|
        /// |10723|load achievement data failed|
        ///
        /// A message of type `MessageType.Achievements_GetDefinitionsByName` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `AchievementDefinitionList`.
        /// Extract the payload from the message handle with `Message.Data`.
        ///
        /// `AchievementDefinitionList` contains the following:
        /// * `Type`: The type of the achievement.
        /// * `Name`: The API name of the achievement.
        /// * `BitfieldLength`: The total bits in the bitfield. For bitfield achievements only.
        /// * `Target`: The number of events to complete for unlocking the achievement. For count or bitfield achievements only.
        /// * `Description`: The description of the achievement.
        /// * `Title`: The display name of the achievement that users see.
        /// * `IsArchived`: Whether the achievement is archived. Archiving will not delete the achievement or users' progress on it.
        /// * `IsSecret`: Whether the achievement is hidden until it is unlocked by users.
        /// * `ID`: The data ID.
        /// * `UnlockedDescription`: The message displayed to users when they unlock the achievement.
        /// * `WritePolicy`: Who are able to write achievement progress.
        /// * `LockedImageURL`: The local path to the image displayed to users before they unlock the achievement.
        /// * `UnlockedImageURL`: The local path to the image displayed to users after they unlock the achievement.
        /// </returns>
        public static Task<AchievementDefinitionList> GetDefinitionsByName(string[] names)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            return new Task<AchievementDefinitionList>(CLIB.ppf_Achievements_GetDefinitionsByName(names));
        }

        /// <summary>Gets the user's progress on specified achievements, including API names,
        /// whether or not the achievements are unlocked, the time at which they were unlocked,
        /// achievement types and, depending on the type, the progress made towards unlocking them, and more.</summary>
        /// <param name="names">The API names of the achievements.</param>
        /// <returns>The request ID of this async function.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10729|invalid api name|
        /// |10730|too many api names|
        /// |10721|invalid request|
        /// |10723|load achievement data failed|
        ///
        /// A message of type `MessageType.Achievements_GetProgressByName` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `AchievementProgressList`.
        /// Extract the payload from the message handle with `Message.Data`.
        ///
        /// `AchievementProgressList` contains the following:
        /// * `ID`: The data ID.
        /// * `Bitfield`: A bitfield displaying the bits unlocked for a bitfield achievement, for example, `1110001`.
        /// * `Count`: The number of events completed for unlocking a count achievement.
        /// * `IsUnlocked`: Whether the achievement is unlocked.
        /// * `Name`: The API name of the achievement.
        /// * `UnlockTime`: The time at which the achievement was unlocked.
        /// * `ExtraData`: Records the key information when unlocking the achievement.
        /// </returns>
        public static Task<AchievementProgressList> GetProgressByName(string[] names)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            return new Task<AchievementProgressList>(CLIB.ppf_Achievements_GetProgressByName(names));
        }

        /// <summary>Unlocks a specified achievement of any type even if the target for
        /// unlocking this achievement is not reached.
        /// </summary>
        /// <param name="name">The API name of the achievement to unlock.</param>
        /// <param name="extraData">Custom extension fields that can be used to record key information when unlocking achievements.</param>
        /// <returns>The request ID of this async function.
        /// | Error Code| Error Message |
        /// |---|---|
        /// |10729|invalid api name|
        /// |10725|extra data too long|
        /// |10720|achievement is not exist|
        /// |10723|load achievement data failed|
        /// |10726|achievement is unreleased|
        /// |10727|achievement is archived|
        /// |10722|no write permission|
        /// |10736|invalid parameter|
        /// |10735|invalid extra data|
        /// |10728|achievement is unlocked|
        /// |10724|save achievement data failed|
        ///
        /// A message of type `MessageType.Achievements_Unlock` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `AchievementUpdate`.
        /// Extract the payload from the message handle with `Message.Data`.
        ///
        /// `AchievementUpdate` contains the following:
        /// * `JustUnlocked`: Whether the achievement has been successfully unlocked.
        /// * `Name`: The API name of the achievement.
        /// </returns>
        public static Task<AchievementUpdate> Unlock(string name, byte[] extraData)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.NotInitializedError);
                return null;
            }
            
            GCHandle hobj = GCHandle.Alloc(extraData, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var result = new Task<AchievementUpdate>(CLIB.ppf_Achievements_Unlock(name, pobj, (uint) (extraData != null ? extraData.Length : 0)));
            if (hobj.IsAllocated)
                hobj.Free();
            return result;
        }
    }
}