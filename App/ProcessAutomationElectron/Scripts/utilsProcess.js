export function retrieveStepVariableList(stepNumber, taskModel, processData) {
    let variableList = [];

    variableList.push(
        ...taskModel.StepList
            .filter(step => step.Number < stepNumber && step.Variable && step.Variable !== "")
            .sort((a, b) => a.Variable.localeCompare(b.Variable))
            .map(step => step.Variable)
    );

    variableList.push(
        ...processData.TaskList
            .filter(task => task.Number < taskModel.Number)
            .sort((a, b) => b.Number - a.Number)
            .flatMap(task =>
                task.StepList
                    .filter(step => step.IsPublicVariable && step.Variable && step.Variable !== "")
                    .map(step => `${step.Variable},${task.Name}`)
            )
    );

    return [...new Set(variableList)].sort((a, b) => a.localeCompare(b));
}