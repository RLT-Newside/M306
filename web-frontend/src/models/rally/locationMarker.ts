export abstract class LocationMarker {
  id: string;
  type: string;

  constructor(id: string, type: string) {
    this.id = id;
    this.type = type;
  }
}

export class IBeaconLocationMarker extends LocationMarker {
  uuid: string;
  major: number;
  minor: number;

  constructor(id: string, type: string, uuid: string, major: number, minor: number) {
    super(id, type);
    this.uuid = uuid;
    this.major = major;
    this.minor = minor;
  }
}

export class QRCodeLocationMarker extends LocationMarker {
  content: string;

  constructor(id: string, type: string, content: string) {
    super(id, type);
    this.content = content;
  }
}
