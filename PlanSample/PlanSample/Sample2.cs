using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;


namespace PlanSample
{
    class Sample2
    {
        public void Do()
        {
            // DI コンテナを生成
            UnityContainer container = new UnityContainer();

            // 名前を指定してインスタンスを登録
            container.RegisterInstance<IAnimal>("Dog", new Dog());
            container.RegisterInstance<IAnimal>("Cat", new Cat());
            
            Person person = new Person();


            // 依存性を注入する
            person = container.BuildUp<Person>(person);

            // ペットを呼ぶ
            person.CallPet();

            Console.ReadLine();
        }

        public void Do2()
        {
            // DI コンテナを生成
            UnityContainer container = new UnityContainer();

            // 名前を指定してインスタンスを登録
            container.RegisterInstance<IAnimal>("Dog", new Dog());
            container.RegisterInstance<IAnimal>("Cat", new Cat());
            
            Person person = new Person();

            // 依存性を注入する
            person = container.BuildUp<Person>(person);

            // ペットを呼ぶ
            person.CallPet();

            Console.ReadLine();
        }
    }

    
    public interface IAnimal
    {
        // 鳴き声を出力する
        void Cry();
    }

    // ネコ
    public class Cat : IAnimal
    {
        public void Cry()
        {
            Console.WriteLine("ニャ〜");
        }
    }

    // イヌ
    public class Dog : IAnimal
    {
        public void Cry()
        {
            Console.WriteLine("バウ！");
        }
    }

    public class Person
    {
        [Dependency("Dog")]
        public IAnimal Pet { get;set; }


        // ペットを呼ぶ
        public void CallPet()
        {
            Pet.Cry();
        }
    }
}
