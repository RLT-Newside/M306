export class RejectReason {
  id: string;
  title: string;
  explanation: string;

  constructor(id: string, title: string, explanation: string) {
    this.id = id;
    this.title = title;
    this.explanation = explanation;
  }
}
