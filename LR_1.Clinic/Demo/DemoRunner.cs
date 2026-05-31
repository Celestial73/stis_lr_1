using LR_1.Clinic.Core;
using LR_1.Clinic.Domain;

namespace LR_1.Clinic.Demo;

public static class DemoRunner
{
    private static int _passCount;
    private static int _failCount;

    public static void Run()
    {
        _passCount = 0;
        _failCount = 0;

        var model = new ClinicModel();

        PrintHeader();
        PrintConcepts(model);
        PrintFrames(model);
        PrintInstanceOfTests(model);
        PrintRelations(model);
        PrintRelationTypeConstraints(model);
        PrintRelationIsaTests(model);
        PrintKripkeScale(model);
        PrintSummary();
    }

    private static void PrintHeader()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine("  ЛР-1: Система вывода с концептами");
        Console.WriteLine("  Предметная область: Медицинская клиника");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine();
    }

    private static void PrintConcepts(ClinicModel model)
    {
        Console.WriteLine("── 1. Концепты и ISA-иерархия ──");
        Console.WriteLine();

        PrintConceptTree(model.Entity, 0, model);
        Console.WriteLine();
    }

    private static void PrintConceptTree(Concept concept, int depth, ClinicModel model)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}{concept.Name}");

        var children = model.KnowledgeBase.Concepts
            .Where(c => ReferenceEquals(c.Parent, concept))
            .OrderBy(c => c.Name);

        foreach (var child in children)
            PrintConceptTree(child, depth + 1, model);
    }

    private static void PrintFrames(ClinicModel model)
    {
        Console.WriteLine("── 2. Экземпляры (фреймы) ──");
        Console.WriteLine();

        foreach (var frame in model.KnowledgeBase.Frames)
        {
            Console.WriteLine($"  {frame.Id} : {frame.Concept.Name}");
            foreach (var slot in frame.Slots)
                Console.WriteLine($"    {slot.Key} = {slot.Value}");
            Console.WriteLine();
        }
    }

    private static void PrintInstanceOfTests(ClinicModel model)
    {
        Console.WriteLine("── 3. Проверка instance-of (транзитивный ISA) ──");
        Console.WriteLine();

        AssertInstanceOf(model.PatientIvanov, model.Patient, true, "прямое совпадение");
        AssertInstanceOf(model.PatientIvanov, model.Participant, true, "Пациент → Участник");
        AssertInstanceOf(model.PatientIvanov, model.Entity, true, "Пациент → Участник → Сущность");
        AssertInstanceOf(model.PatientIvanov, model.Doctor, false, "отрицательный тест");

        AssertInstanceOf(model.DoctorSidorov, model.Doctor, true, "прямое совпадение");
        AssertInstanceOf(model.DoctorSidorov, model.Participant, true, "Врач → Участник");
        AssertInstanceOf(model.DoctorSidorov, model.Entity, true, "Врач → Участник → Сущность");

        AssertInstanceOf(model.ServiceConsult, model.Service, true, "прямое совпадение");
        AssertInstanceOf(model.ServiceConsult, model.Entity, true, "Услуга → Сущность");

        AssertInstanceOf(model.Appointment001, model.Appointment, true, "прямое совпадение");
        AssertInstanceOf(model.Appointment001, model.Entity, true, "Приём → Сущность");
        AssertInstanceOf(model.Appointment001, model.Participant, false, "отрицательный тест");

        Console.WriteLine();
    }

    private static void AssertInstanceOf(Frame frame, Concept concept, bool expected, string comment)
    {
        var actual = KnowledgeBase.InstanceOf(frame, concept);

        if (actual == expected)
        {
            _passCount++;
            Console.WriteLine($"  [PASS] {frame.Id} instance-of {concept.Name}  ({comment})");
        }
        else
        {
            _failCount++;
            Console.WriteLine($"  [FAIL] {frame.Id} instance-of {concept.Name}  ожидалось {expected}, получено {actual}");
        }
    }

    private static void PrintRelations(ClinicModel model)
    {
        Console.WriteLine("── 4. Связи между экземплярами ──");
        Console.WriteLine();
        Console.WriteLine("  Сигнатуры:");
        Console.WriteLine($"    {model.Registers.FormatSignature()}");
        Console.WriteLine($"    {model.Pays.FormatSignature()}");
        Console.WriteLine($"    {model.Provides.FormatSignature()}");
        Console.WriteLine($"    {model.Contains.FormatSignature()}");
        Console.WriteLine();
        Console.WriteLine("  Экземпляры:");
        Console.WriteLine($"  {model.Registers.Name}({model.PatientIvanov.Id}, {model.Appointment001.Id})");
        Console.WriteLine($"  {model.Provides.Name}({model.DoctorSidorov.Id}, {model.Appointment001.Id})");
        Console.WriteLine($"  {model.Contains.Name}({model.Appointment001.Id}, {model.ServiceConsult.Id})");
        Console.WriteLine($"  {model.Pays.Name}({model.PatientIvanov.Id}, {model.Appointment001.Id})");
        Console.WriteLine();
    }

    private static void PrintRelationTypeConstraints(ClinicModel model)
    {
        Console.WriteLine("── 5. Ограничения типов на связях ──");
        Console.WriteLine();

        AssertRelationValid(
            model.Pays,
            [model.PatientIvanov, model.Appointment001],
            true,
            "ОПЛАЧИВАЕТ(Пациент, Приём) — допустимо");

        AssertRelationValid(
            model.Pays,
            [model.PatientIvanov, model.DoctorSidorov],
            false,
            "ОПЛАЧИВАЕТ(Пациент, Врач) — недопустимо");

        AssertRelationValid(
            model.Provides,
            [model.DoctorSidorov, model.Appointment001],
            true,
            "ОКАЗЫВАЕТ(Врач, Приём) — допустимо");

        AssertRelationValid(
            model.Contains,
            [model.Appointment001, model.ServiceConsult],
            true,
            "СОДЕРЖИТ(Приём, Услуга) — допустимо");

        AssertAddFactRejected(
            model,
            model.Pays,
            model.PatientIvanov,
            model.DoctorSidorov,
            "AddFact отклоняет ОПЛАЧИВАЕТ(patient_ivanov, doctor_sidorov)");

        Console.WriteLine();
    }

    private static void AssertRelationValid(Relation relation, Frame[] arguments, bool expectedValid, string comment)
    {
        var valid = relation.TryValidateArguments(arguments, out var error);

        if (valid == expectedValid)
        {
            _passCount++;
            Console.WriteLine($"  [PASS] {comment}");
            if (!expectedValid && error is not null)
                Console.WriteLine($"         → {error}");
        }
        else
        {
            _failCount++;
            Console.WriteLine($"  [FAIL] {comment}  ожидалось valid={expectedValid}, получено {valid}");
        }
    }

    private static void AssertAddFactRejected(
        ClinicModel model,
        Relation relation,
        Frame arg1,
        Frame arg2,
        string comment)
    {
        try
        {
            model.W0.AddFact(relation, arg1, arg2);
            _failCount++;
            Console.WriteLine($"  [FAIL] {comment}  ожидалось исключение, факт был добавлен");
        }
        catch (ArgumentException ex)
        {
            _passCount++;
            Console.WriteLine($"  [PASS] {comment}");
            Console.WriteLine($"         → {ex.Message}");
        }
    }

    private static void PrintRelationIsaTests(ClinicModel model)
    {
        Console.WriteLine("── 6. ISA на связях (п.7–8, instance-of для связей) ──");
        Console.WriteLine();

        AssertRelationIsA(model.Registers, model.PatientAction, true, "ЗАПИСЫВАЕТ → ДействиеПациента");
        AssertRelationIsA(model.Pays, model.PatientAction, true, "ОПЛАЧИВАЕТ → ДействиеПациента");
        AssertRelationIsA(model.Provides, model.DoctorAction, true, "ОКАЗЫВАЕТ → ДействиеВрача");
        AssertRelationIsA(model.Registers, model.DoctorAction, false, "отрицательный тест");
        AssertRelationIsA(model.Contains, model.PatientAction, false, "СОДЕРЖИТ не ISA ДействиеПациента");

        Console.WriteLine();
    }

    private static void AssertRelationIsA(Relation relation, Relation expectedParent, bool expected, string comment)
    {
        var actual = relation.IsA(expectedParent);

        if (actual == expected)
        {
            _passCount++;
            Console.WriteLine($"  [PASS] {relation.Name} ISA {expectedParent.Name}  ({comment})");
        }
        else
        {
            _failCount++;
            Console.WriteLine($"  [FAIL] {relation.Name} ISA {expectedParent.Name}  ожидалось {expected}, получено {actual}");
        }
    }

    private static void PrintKripkeScale(ClinicModel model)
    {
        Console.WriteLine("── 7. Шкала Крипке (возможные миры) ──");
        Console.WriteLine();

        Console.WriteLine("  Достижимость: W0 → W1 → W2 → W3");
        Console.WriteLine();

        foreach (var world in model.KripkeScale.Worlds)
        {
            var status = model.GetAppointmentStatus(world);
            Console.WriteLine($"  [{world.Name}]  статус appt_001 = {status}");

            if (world.Facts.Count == 0)
            {
                Console.WriteLine("    (фактов нет)");
            }
            else
            {
                foreach (var fact in world.Facts)
                    Console.WriteLine($"    {fact}");
            }

            Console.WriteLine();
        }

        var reachable = model.KripkeScale.ReachableFrom(model.W0).Select(w => w.Name.Split(':')[0]).ToList();
        Console.WriteLine($"  ReachableFrom(W0): {string.Join(" → ", reachable)}");

        var accessibleFromW1 = model.KripkeScale.GetAccessible(model.W1).Select(w => w.Name.Split(':')[0]);
        Console.WriteLine($"  Accessible(W1): {string.Join(", ", accessibleFromW1)}");
        Console.WriteLine();
    }

    private static void PrintSummary()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine($"  Итого: {_passCount} PASS, {_failCount} FAIL");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
    }
}
