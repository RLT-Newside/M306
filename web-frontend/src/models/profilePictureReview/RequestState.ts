export enum RequestState {
  Pending,
  Fulfilled,
  Expired,
}

export function getRequestStateLabel(state: RequestState): string {
  switch (state) {
    case RequestState.Pending:
      return 'pendent';
    case RequestState.Fulfilled:
      return 'abgeschlossen';
    case RequestState.Expired:
      return 'abgelaufen';
    default:
      return 'unbekannt';
  }
}
