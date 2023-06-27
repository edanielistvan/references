import java.util.ArrayList;

public class Manager extends Employee 
{
	ArrayList<Employee> employees;
	
	public Manager(String name, int salary)
	{
		super(name, salary);
		
		employees = new ArrayList<Employee>();
	}
	
	public void addEmployee(Employee emp)
	{
		employees.add(emp);
	}
	
	public void removeEmployee(Employee emp)
	{
		employees.remove(emp);
	}
	
	@Override
	public int getSalary()
	{
		double bonus = 0;
		
		for(int i = 0; i < employees.size(); i++)
		{
			bonus += employees.get(i).getSalary();
		}
		
		return salary + (int)Math.floor(bonus * 0.05);
	}
}