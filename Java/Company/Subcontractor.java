
public class Subcontractor implements SalariedEntity
{
	private long taxid;
	private int salary;
	
	public Subcontractor(long taxid, int salary)
	{
		this.taxid = taxid;
		this.salary = salary;
	}
	
	@Override
	public int getSalary() {
		return salary;
	}
	
	@Override
	public void raiseSalary(double percentage)
	{
		System.out.println("You can't raise the salary of contractor: " + taxid);
	}

}
