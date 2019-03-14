using System;
using System.ServiceModel;

namespace Calculator
{
    [ServiceContract]
    public class Service : IService
    {
        [OperationContract]
        public int Add(int num1, int num2)
        {
            return num1 + num2;
        }

        [OperationContract]
        public string Help()
        {
            return $"{Environment.MachineName}";
        }
    }

    public interface IService
    {
        int Add(int num1, int num2);

        string Help();
    }
}