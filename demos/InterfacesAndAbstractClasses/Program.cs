// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var mac = new Cat();
mac.Eat("cheeseburger");
Console.WriteLine($"Mac has {mac.Legs:0.000} legs");
Console.WriteLine($"Mac says: {mac.Talk()}");

GreetPet(mac);

var fred = new Fish();
FeedFriesToAnimal(fred);
FeedFriesToFish(fred);
FeedSodaToAnimal(fred);
FeedSodaToFish(fred);

// GreetPet(fred);

void FeedFriesToAnimal(Animal a) => a.Eat("french fries");
void FeedFriesToFish(Fish f) => f.Eat("french fries");

void FeedSodaToAnimal(Animal a) => a.Drink("soda");
void FeedSodaToFish(Fish f) => f.Drink("soda");

void GreetPet(Pet p) {
	p.Smile();
	p.Snuggle();
	p.DestroyTheFurniture();
}

public interface Pet {
	void Smile();
	void Snuggle();

	void DestroyTheFurniture() {
		Console.WriteLine("I'm gonna ruin your sofa! (Interface pet)");
	}
}

public abstract class Animal {
	public abstract int Legs { get; }
	public abstract string Talk();
	public void Eat(string food) {
		Console.WriteLine("Om nom nom!" + food);
	}

	public void DestroyTheFurniture() {
		Console.WriteLine("I'm gonna ruin your sofa! (Animal version)");
	}

	public virtual void Drink(string food) {
		Console.WriteLine("Om nom nom!" + food);
	}

}

public class Cat : Animal, Pet {
	public override int Legs => 4;

	public void Smile() => Console.WriteLine("Smiley cat!");
	public void Snuggle() => Console.WriteLine("purr purr");

	public override string Talk() => "Miaow!";
}

public class Fish : Animal {
	public override int Legs => 0;

	public override string Talk() => "glug";

	public new void Eat(string food) {
		Console.WriteLine($"fish don't eat {food}");
	}

	public override void Drink(string liquid) {
		Console.WriteLine($"Fish don't drink {liquid}");
	}
}
