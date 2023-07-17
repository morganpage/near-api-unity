using NearClientUnity.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NearClientUnity
{
  public class Action
  {
    private readonly JObject _args;
    private readonly ActionType _type;

    public Action(ActionType type, JObject args)
    {
      _type = type;
      _args = args;
    }

    public JObject Args => _args;

    public ActionType Type => _type;

    public static Action AddKey(PublicKey publicKey, AccessKey accessKey)
    {
      JObject args = new JObject();
      args["PublicKey"] = (JToken)publicKey;
      args["AccessKey"] = (JToken)accessKey;
      return new Action(ActionType.AddKey, args);
    }

    public static Action CreateAccount()
    {
      return new Action(ActionType.CreateAccount, null);
    }

    public static Action DeleteAccount(string beneficiaryId)
    {
      JObject args = new JObject();
      args["BeneficiaryId"] = beneficiaryId;
      return new Action(ActionType.DeleteAccount, args);
    }

    public static Action DeleteKey(PublicKey publicKey)
    {
      JObject args = new JObject();
      args["PublicKey"] = (JToken)publicKey;
      return new Action(ActionType.DeleteKey, args);
    }

    public static Action DeployContract(byte[] code)
    {
      JObject args = new JObject();
      args["Code"] = code;
      return new Action(ActionType.DeployContract, args);
    }

    public static Action FromByteArray(byte[] rawBytes)
    {
      using (var ms = new MemoryStream(rawBytes))
      {
        return FromStream(ms);
      }
    }

    public static Action FromStream(MemoryStream stream)
    {
      return FromRawDataStream(stream);
    }

    public static Action FromStream(ref MemoryStream stream)
    {
      return FromRawDataStream(stream);
    }

    public static Action FunctionCall(string methodName, byte[] methodArgs, ulong? gas, UInt128 deposit)
    {
      JObject args = new JObject();
      args["MethodName"] = methodName;
      args["MethodArgs"] = methodArgs;
      args["Gas"] = gas;
      args["Deposit"] = (ulong)deposit;
      return new Action(ActionType.FunctionCall, args);
    }

    public static Action Stake(UInt128 stake, PublicKey publicKey)
    {
      JObject args = new JObject();
      args["Stake"] = (ulong)stake;
      args["PublicKey"] = (JToken)publicKey;
      return new Action(ActionType.Stake, args);
    }

    public static Action Transfer(UInt128 deposit)
    {
      JObject args = new JObject();
      args["Deposit"] = (ulong)deposit;
      return new Action(ActionType.Transfer, args);
    }

    public byte[] ToByteArray()
    {
      using (var ms = new MemoryStream())
      {
        using (var writer = new NearBinaryWriter(ms))
        {
          writer.Write((byte)_type);

          switch (_type)
          {
            case ActionType.AddKey:
              {
                writer.Write(((PublicKey)_args["PublicKey"]).ToByteArray());
                writer.Write(((AccessKey)_args["AccessKey"]).ToByteArray());
                return ms.ToArray();
              }
            case ActionType.DeleteKey:
              {
                writer.Write(((PublicKey)_args["PublicKey"]).ToByteArray());
                return ms.ToArray();
              }
            case ActionType.CreateAccount:
              {
                return ms.ToArray();
              }
            case ActionType.DeleteAccount:
              {
                writer.Write((string)_args["BeneficiaryId"]);
                return ms.ToArray();
              }
            case ActionType.DeployContract:
              {
                writer.Write((uint)_args["Code"].ToObject<byte[]>().Length);
                writer.Write((byte[])_args["Code"].ToObject<byte[]>());
                return ms.ToArray();
              }
            case ActionType.FunctionCall:
              {
                writer.Write((string)_args["MethodName"]);
                writer.Write((uint)(_args["MethodArgs"].ToObject<byte[]>()).Length);
                writer.Write((byte[])_args["MethodArgs"].ToObject<byte[]>());
                writer.Write((ulong)_args["Gas"]);
                writer.Write((UInt128)(_args["Deposit"].ToObject<UInt128>()));//Must be UInt128
                return ms.ToArray();
              }
            case ActionType.Stake:
              {
                writer.Write((UInt128)_args["Stake"].ToObject<UInt128>());
                writer.Write(((PublicKey)_args["PublicKey"]).ToByteArray());
                return ms.ToArray();
              }
            case ActionType.Transfer:
              {
                writer.Write((UInt128)_args["Deposit"].ToObject<UInt128>());
                return ms.ToArray();
              }
            default:
              throw new NotSupportedException("Unsupported action type");
          }
        }
      }
    }

    private static Action FromRawDataStream(MemoryStream stream)
    {
      using (var reader = new NearBinaryReader(stream, true))
      {
        var actionType = (ActionType)reader.ReadByte();

        switch (actionType)
        {
          case ActionType.AddKey:
            {
              JObject args = new JObject();
              args["PublicKey"] = (JToken)PublicKey.FromStream(ref stream);
              args["AccessKey"] = (JToken)AccessKey.FromStream(ref stream);
              return new Action(ActionType.AddKey, args);
            }
          case ActionType.DeleteKey:
            {
              JObject args = new JObject();
              args["PublicKey"] = (JToken)PublicKey.FromStream(ref stream);
              return new Action(ActionType.DeleteKey, args);
            }
          case ActionType.CreateAccount:
            {
              return new Action(ActionType.CreateAccount, null);
            }
          case ActionType.DeleteAccount:
            {
              JObject args = new JObject();
              args["BeneficiaryId"] = reader.ReadString();
              return new Action(ActionType.DeleteAccount, args);
            }
          case ActionType.DeployContract:
            {
              JObject args = new JObject();

              var byteCount = reader.ReadUInt();

              var code = new List<byte>();

              for (var i = 0; i < byteCount; i++)
              {
                code.Add(reader.ReadByte());
              }

              args["Code"] = code.ToArray();
              return new Action(ActionType.DeployContract, args);
            }
          case ActionType.FunctionCall:
            {
              JObject args = new JObject();
              var methodName = reader.ReadString();
              var methodArgsCount = reader.ReadUInt();
              var methodArgs = new List<byte>();
              for (var i = 0; i < methodArgsCount; i++)
              {
                methodArgs.Add(reader.ReadByte());
              }
              var gas = reader.ReadULong();
              var deposit = reader.ReadUInt128();
              args["MethodName"] = methodName;
              args["MethodArgs"] = methodArgs.ToArray();
              args["Gas"] = gas;
              args["Deposit"] = (ulong)deposit;
              return new Action(ActionType.FunctionCall, args);
            }
          case ActionType.Stake:
            {
              JObject args = new JObject();

              var stake = reader.ReadUInt128();

              var publicKey = PublicKey.FromStream(ref stream);

              args["Stake"] = (ulong)stake;
              args["PublicKey"] = (JToken)publicKey;

              return new Action(ActionType.Stake, args);
            }
          case ActionType.Transfer:
            {
              JObject args = new JObject();

              var deposit = reader.ReadUInt128();

              args["Deposit"] = (ulong)deposit;

              return new Action(ActionType.Transfer, args);
            }
          default:
            throw new NotSupportedException("Unsupported action type");
        }
      }
    }
  }
}