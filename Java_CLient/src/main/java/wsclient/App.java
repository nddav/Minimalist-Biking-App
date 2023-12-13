package wsclient;

import org.apache.maven.plugin.MojoExecutionException;

import java.util.Scanner;

import static org.glassfish.pfl.basic.logex.OperationTracer.exit;


public class App 
{
    public static void main( String[] args ) throws MojoExecutionException {

        String originAdress,destAdress;
        System.out.println("Client is started");

        // What your client will do after launch
        Service1 service1 = new Service1();
        Interface1 proxy = service1.getBasicHttpBindingInterface1();

        Scanner scanner = new Scanner(System.in);
        System.out.println("Veuillez rentrer une adresse de départ au format suivant : 40 AVENUE DE MURET Toulouse");
        originAdress = scanner.nextLine();

        // Just in case the client messes up
        while(originAdress.equals("")){
            System.out.println("Veuillez rentrer une adresse de départ au format suivant : 40 AVENUE DE MURET Toulouse");
            originAdress = scanner.nextLine();
        }

        System.out.println("Vous avez rentré l'adresse suivante : " + originAdress);

        System.out.println("Veuillez rentrer une adresse de d'arrivée au format suivant : 41 AV. ROGER SALENGRO Lyon");
        destAdress = scanner.nextLine();

        // Just in case the client messes up
        while(destAdress.equals("")){
            System.out.println("Veuillez rentrer une adresse de départ au format suivant : 40 AVENUE DE MURET Toulouse");
            destAdress = scanner.nextLine();
        }

        System.out.println("Vous avez rentré l'adresse suivante : " + destAdress + "\nVeuillez patienter...");
        scanner.close();

        // Call the big method
        ArrayOfStep steps = proxy.getItinerary(originAdress,destAdress);

        try{
            steps.step.size();
        } catch (NullPointerException e){
            System.out.println("It is not worth taking a bike you must walk");
            return;
        }

        // Printing of the itinerary for the client
        for(int i = 0;i<steps.step.size();i++)
        {
            System.out.println("In " + steps.step.get(i).distance + "M, " + steps.step.get(i).instruction.getValue());
        }
        System.out.println("Fin du trajet");
    }
}
