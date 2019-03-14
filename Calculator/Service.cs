using System;
using System.ServiceModel;

namespace Calculator
{
    public class Service : IService
    {
        public int Add(int num1, int num2)
        {
            return num1 + num2;
        }

        public string Help()
        {
            return $"{Environment.MachineName}";
        }
    }

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        int Add(int num1, int num2);

        [OperationContract]
        string Help();
    }
}