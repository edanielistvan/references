import java.util.ArrayList;

public class Company
{
	ArrayList<SalariedEntity> entities;
	
	public Company()
	{
		entities = new ArrayList<SalariedEntity>();
	}
	
	public void addEntity(SalariedEntity entity)
	{
		entities.add(entity);
	}
	
	public void removeEntity(SalariedEntity entity)
	{
		entities.remove(entity);
	}
	
	public void raiseSalary(double percentage)
	{
		if (percentage < 0) throw new IllegalArgumentException();
		
		for (SalariedEntity emp : entities)
		{
			if (emp instanceof Employee) emp.raiseSalary(percentage);
		}
	}
}
