namespace IncidentRecord;

public record Nmi
{
    public required string nmi;
    public required string streetName;
    public required string nmiState;
    public required string suburb;
    public required string postcode;

}

public record Incident
{
    public required string id;
    public required string type;
    public required string cause;
    public required string delayReason;
    public required string etrUpdateReason;
    public required string emergencyFlag;
    public required string status;
    public required string incidentStatus;
    public required string statusLastUpdated;
    public required string unplannedStartTime;
    public required string plannedStartTime;
    public required string plannedEndTime;
    public required string plannedDelayed;
    public required string estimatedTimeToAssessment;
    public required string initialEstimatedTimeToRestoration;
    public required string latestEstimatedTimeToRestoration;
    public required string actualTimeOfRestoration;
    public required string mergedIncidentId;
    public required string priority;
    public required string longitude;
    public required string latitude;
    public required string incidentType;
    public required string categoryId;
    public required string IncidentCreated;
    public required string IncidentLastUpdated;
    public required List<Nmi> impactedNMI;
}