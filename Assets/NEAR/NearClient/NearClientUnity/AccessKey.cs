using NearClientUnity.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine;

namespace NearClientUnity
{
  public class AccessKey
  {
    public ulong Nonce { get; set; }
    public AccessKeyPermission Permission { get; set; }

    public static AccessKey FromByteArray(byte[] rawBytes)
    {
      using (var ms = new MemoryStream(rawBytes))
      {
        return FromStream(ms);
      }
    }

    public static AccessKey FromDynamicJsonObject(JObject jsonObject)
    {
      try
      {
        bool isFullAccess = jsonObject["permission"].ToString() == "FullAccess";
        if (isFullAccess)
        {
          return new AccessKey
          {
            Nonce = (ulong)(JToken)jsonObject["nonce"],
            Permission = new AccessKeyPermission
            {
              PermissionType = AccessKeyPermissionType.FullAccessPermission,
              FullAccess = new FullAccessPermission()
            }
          };
        }
        else
        {
          ulong rawNonce = (ulong)jsonObject["nonce"];
          string rawAllowance = (string)(jsonObject["permission"]["FunctionCall"]["allowance"] ?? null);
          UInt128? allowance = rawAllowance != null ? UInt128.Parse(rawAllowance) : null;
          string[] rawMethodNames = jsonObject["permission"]["FunctionCall"]["method_names"].ToObject<string[]>();
          string rawReceiverId = (string)jsonObject["permission"]["FunctionCall"]["receiver_id"];
          return new AccessKey
          {
            Nonce = rawNonce,
            Permission = new AccessKeyPermission
            {
              PermissionType = AccessKeyPermissionType.FunctionCallPermission,
              FunctionCall = new FunctionCallPermission()
              {
                Allowance = allowance,
                MethodNames = rawMethodNames,
                ReceiverId = rawReceiverId,
              }
            }
          };
        }

      }
      catch (System.Exception e)
      {
        Debug.Log("AccessKey.cs: FromDynamicJsonObject: " + e.Message);
        throw;
      }


    }

    public static AccessKey FromStream(MemoryStream stream)
    {
      return FromRawDataStream(stream);
    }

    public static AccessKey FromStream(ref MemoryStream stream)
    {
      return FromRawDataStream(stream);
    }

    public static AccessKey FullAccessKey()
    {
      var key = new AccessKey
      {
        Nonce = 0,
        Permission = new AccessKeyPermission
        {
          PermissionType = AccessKeyPermissionType.FullAccessPermission,
          FullAccess = new FullAccessPermission()
        }
      };
      return key;
    }

    public static AccessKey FunctionCallAccessKey(string receiverId, string[] methodNames, UInt128? allowance)
    {
      var key = new AccessKey
      {
        Nonce = 0,
        Permission = new AccessKeyPermission
        {
          PermissionType = AccessKeyPermissionType.FunctionCallPermission,
          FunctionCall = new FunctionCallPermission
          {
            ReceiverId = receiverId,
            Allowance = allowance,
            MethodNames = methodNames
          }
        }
      };
      return key;
    }

    public static AccessKey FunctionCallAccessKey(string receiverId, string[] methodNames)
    {
      var key = new AccessKey
      {
        Nonce = 0,
        Permission = new AccessKeyPermission
        {
          PermissionType = AccessKeyPermissionType.FunctionCallPermission,
          FunctionCall = new FunctionCallPermission
          {
            ReceiverId = receiverId,
            MethodNames = methodNames
          }
        }
      };
      return key;
    }

    public byte[] ToByteArray()
    {
      using (var ms = new MemoryStream())
      {
        using (var writer = new NearBinaryWriter(ms))
        {
          writer.Write(Nonce);
          writer.Write(Permission.ToByteArray());
          return ms.ToArray();
        }
      }
    }

    private static AccessKey FromRawDataStream(MemoryStream stream)
    {
      using (var reader = new NearBinaryReader(stream, true))
      {
        var nonce = reader.ReadULong();
        var permission = AccessKeyPermission.FromStream(ref stream);

        return new AccessKey()
        {
          Nonce = nonce,
          Permission = permission
        };
      }
    }

    public static explicit operator JToken(AccessKey v)
    {
      return v.ToByteArray();
    }
    public static explicit operator AccessKey(JToken v)
    {
      return FromByteArray(v.ToObject<byte[]>());
    }
  }
}