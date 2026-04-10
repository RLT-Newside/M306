export enum ValidationState {
  Pending,
  Passed,
  Failed,
}

export function getValidationStateLabel(state: ValidationState): string {
  switch (state) {
    case ValidationState.Pending:
      return 'ausstehend';
    case ValidationState.Passed:
      return 'erfolgreich';
    case ValidationState.Failed:
      return 'fehlgeschlagen';
    default:
      return 'unbekannt';
  }
}
