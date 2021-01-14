using UnityEngine;
using static System.Math;

public class Genes
{

    const float mutationChance = .3f;
    const float maxMutationAmount = .3f;
    static readonly System.Random prng = new System.Random();

    //Genes
    public readonly float[] values;


    public readonly string gender;
    public readonly float health;
    public readonly float attackDamage;
    public readonly float desirability;
    public readonly float gestationPeriod;
    public readonly float speed;
    public readonly float stamina; 
    public readonly float reproductionUrge;
    public readonly float metabolism;
 
    public Genes(float[] values)
    {
        if (RandomValue() < 0.5f)
        {
            gender = "male";
            desirability = values[0];
        }
        else
        {
            gender = "female";
            desirability = values[0];
        }
        gestationPeriod = values[1];

        reproductionUrge = values[2];
        speed = values[3];
        attackDamage = values[4];
        stamina = values[5];
        metabolism = values[6];
        health = values[7];
        // Debug.Log(reproductionUrge);
        this.values = values;
    }

    public static Genes RandomGenes(int num)
    {
        float[] values = new float[num];
        for (int i = 0; i < num; i++)
        {
         //   Debug.Log("ASSIGNED GENES");
            values[i] = RandomValue();
        }
        return new Genes(values);
    }

    public static Genes InheritedGenes(Genes mother, Genes father)
    {
        //sets a new float with the amount of genes mother had
        float[] values = new float[mother.values.Length];
     
        for(int i = 0; i < values.Length; i++)
        {
            values[i] = InheritGene( mother.values[i], father.values[i]);

            
        }

        Genes genes = new Genes(values);

        return genes;
    }

    static float InheritGene(float mother, float father)
    {

        float gene = (RandomValue() < 0.5) ? mother : father;


        if(RandomValue() < mutationChance)
        {

            float mutateAmount = RandomGaussian() * maxMutationAmount;
            gene += mutateAmount;
            Debug.Log("MUTATION OCCURED");
        }
      
        return gene;

    }

    static float RandomValue()
    {
        return (float)prng.NextDouble();
    }

    static float RandomGaussian()
    {
        double u1 = 1 - prng.NextDouble();
        double u2 = 1 - prng.NextDouble();
        double randStdNormal = Sqrt(-2 * Log(u1)) * Sin(2 * PI * u2);
        return (float)randStdNormal;
    }
}