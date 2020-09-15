using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace PlanSample
{
    class Sample1
    {
        public void Do()
        {
            // DI コンテナを生成
            var container = new UnityContainer();

            // 名前を指定してインスタンスを登録
            container.RegisterInstance<IMasterManager>("Customer", new CustomerManager());
            container.RegisterInstance<IMasterManager>("Supplier", new SupplierManager());

            // 名前を指定せずにインスタンスを登録
            container.RegisterInstance<IMasterManager>(new ProductManager());

            //// 得意先を取得
            //IMasterManager manager = container.Resolve<IMasterManager>("Customer");
            //Console.WriteLine(manager.Read().DataSetName);

            //// 仕入先を取得
            //manager = container.Resolve<IMasterManager>("Supplier");
            //Console.WriteLine(manager.Read().DataSetName);

            //// 名前を指定せずにインスタンスの取得を試みる
            //manager = container.Resolve<IMasterManager>();
            //Console.WriteLine(manager.Read().DataSetName);

            // 登録したインスタンスの中から IMasterManager を実装するものをすべて取得する
            IEnumerable<IMasterManager> managers = container.ResolveAll<IMasterManager>();
            foreach (IMasterManager manager in managers)
            {
                Console.WriteLine(manager.Read().DataSetName);
            }

            Console.ReadLine();
        }

    }

    public class CustomerManager : IMasterManager
    {
        public DataSet Read()
        {
            return new DataSet("Customer");
        }
    }

    public interface IMasterManager
    {
        DataSet Read();
    }

    internal class ProductManager : IMasterManager
    {
        public DataSet Read()
        {
            return new DataSet("Product");
        }
    }
    internal class SupplierManager : IMasterManager
    {
        public DataSet Read()
        {
            return new DataSet("Supplier");
        }
    }
}
