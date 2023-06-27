public class Main 
{
	public static void main(String[] args) 
	{
		Company company = new Company();
		
		Subordinate emp = new Subordinate("Sanyi",1000);
		
		System.out.println(emp.getSalary());
		
		emp.raiseSalary(1.5);
		System.out.println(emp.getSalary());
		
		try
		{
			emp.raiseSalary(-1.5);
		}
		catch(IllegalArgumentException ex)
		{
			System.out.println("Raised exception.");
		}
		
		Manager man = new Manager("Laci",5000);
		
		for (int i = 0; i < 10; i++)
		{
			Subordinate temp = new Subordinate(""+i,(i+1)*100); 
			
			man.addEmployee(temp);
			company.addEntity(temp);
		}
		
		man.addEmployee(emp);
		company.addEntity(emp);
		System.out.println(man.getSalary());
		
		man.removeEmployee(emp);		
		company.removeEntity(emp);
		System.out.println(man.getSalary());
		
		Subcontractor cont = new Subcontractor(74434742352l, 1000);
		company.addEntity(cont);
		company.addEntity(emp);
		
		System.out.println(cont.getSalary());
		
		company.raiseSalary(1.05);
		
		System.out.println(cont.getSalary());
		System.out.println(emp.getSalary());
	}
}
