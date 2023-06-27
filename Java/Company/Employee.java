
public abstract class Employee implements SalariedEntity {
	private String name;
	protected int salary;
	
	public Employee(String name, int salary)
	{
		this.name = name;
		this.salary = salary;
	}
	
	public String getName()
	{
		return name;
	}
	
	public abstract int getSalary();
	
	public void raiseSalary(double percentage)
	{
		if (percentage < 0) throw new IllegalArgumentException();
		
		this.salary *= percentage;
	}
}
