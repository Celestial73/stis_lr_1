using LR_1.Clinic.Core;

namespace LR_1.Clinic.Domain;

public sealed class ClinicModel
{
    public Concept Entity { get; }
    public Concept Participant { get; }
    public Concept Patient { get; }
    public Concept Doctor { get; }
    public Concept Service { get; }
    public Concept Appointment { get; }

    public Relation Registers { get; }
    public Relation Provides { get; }

    public Relation PatientAction { get; }
    public Relation DoctorAction { get; }
    public Relation Contains { get; }
    public Relation Pays { get; }

    public Frame PatientIvanov { get; }
    public Frame DoctorSidorov { get; }
    public Frame ServiceConsult { get; }
    public Frame Appointment001 { get; }

    public KnowledgeBase KnowledgeBase { get; }
    public KripkeScale KripkeScale { get; }
    public PossibleWorld W0 { get; }
    public PossibleWorld W1 { get; }
    public PossibleWorld W2 { get; }
    public PossibleWorld W3 { get; }

    public ClinicModel()
    {
        KnowledgeBase = new KnowledgeBase();

        Entity = KnowledgeBase.RegisterConcept("Сущность");
        Participant = KnowledgeBase.RegisterConcept("Участник", Entity);
        Patient = KnowledgeBase.RegisterConcept("Пациент", Participant);
        Doctor = KnowledgeBase.RegisterConcept("Врач", Participant);
        Service = KnowledgeBase.RegisterConcept("Услуга", Entity);
        Appointment = KnowledgeBase.RegisterConcept("Приём", Entity);

        PatientAction = new Relation("ДействиеПациента");
        DoctorAction = new Relation("ДействиеВрача");
        Registers = new Relation("ЗАПИСЫВАЕТ", PatientAction, Patient, Appointment);
        Pays = new Relation("ОПЛАЧИВАЕТ", PatientAction, Patient, Appointment);
        Provides = new Relation("ОКАЗЫВАЕТ", DoctorAction, Doctor, Appointment);
        Contains = new Relation("СОДЕРЖИТ", argumentTypes: [Appointment, Service]);

        PatientIvanov = KnowledgeBase.RegisterFrame("patient_ivanov", Patient, new Dictionary<string, object>
        {
            ["имя"] = "Иванов",
            ["полис"] = "12345"
        });

        DoctorSidorov = KnowledgeBase.RegisterFrame("doctor_sidorov", Doctor, new Dictionary<string, object>
        {
            ["имя"] = "Доктор_Сидоров",
            ["специализация"] = "терапевт"
        });

        ServiceConsult = KnowledgeBase.RegisterFrame("service_consult", Service, new Dictionary<string, object>
        {
            ["название"] = "Консультация_терапевта",
            ["тип"] = "консультация",
            ["цена"] = 1500
        });

        Appointment001 = KnowledgeBase.RegisterFrame("appt_001", Appointment, new Dictionary<string, object>
        {
            ["статус"] = "—",
            ["дата"] = new DateOnly(2026, 5, 31),
            ["кабинет"] = "Кабинет_12"
        });

        KripkeScale = BuildKripkeScale();
        W0 = KripkeScale.Worlds[0];
        W1 = KripkeScale.Worlds[1];
        W2 = KripkeScale.Worlds[2];
        W3 = KripkeScale.Worlds[3];
    }

    private KripkeScale BuildKripkeScale()
    {
        var scale = new KripkeScale();

        var w0 = scale.AddWorld("W0: начальное");
        var w1 = scale.AddWorld("W1: записан");
        var w2 = scale.AddWorld("W2: оплачен");
        var w3 = scale.AddWorld("W3: оказан");

        scale.AddAccessibility(w0, w1);
        scale.AddAccessibility(w1, w2);
        scale.AddAccessibility(w2, w3);

        w1.AddFact(Registers, PatientIvanov, Appointment001);
        w1.AddFact(Contains, Appointment001, ServiceConsult);

        w2.AddFact(Registers, PatientIvanov, Appointment001);
        w2.AddFact(Contains, Appointment001, ServiceConsult);
        w2.AddFact(Pays, PatientIvanov, Appointment001);

        w3.AddFact(Registers, PatientIvanov, Appointment001);
        w3.AddFact(Contains, Appointment001, ServiceConsult);
        w3.AddFact(Pays, PatientIvanov, Appointment001);
        w3.AddFact(Provides, DoctorSidorov, Appointment001);

        return scale;
    }

    public string GetAppointmentStatus(PossibleWorld world)
    {
        if (world.HasFact(Provides, DoctorSidorov, Appointment001))
            return "оказан";
        if (world.HasFact(Pays, PatientIvanov, Appointment001))
            return "оплачен";
        if (world.HasFact(Registers, PatientIvanov, Appointment001))
            return "записан";
        return "—";
    }

    public IEnumerable<Concept> GetConceptHierarchy(Concept concept)
    {
        for (Concept? current = concept; current is not null; current = current.Parent)
            yield return current;
    }
}
