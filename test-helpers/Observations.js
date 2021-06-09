import * as snomedCodes from './snomedCodes';
import { createBundle, createObservation } from './fhir';

export class Observations {
  constructor(observations) {
    this.observations = new Map();

    Object.assign(this, observations);
  }

  get(observationType) {
    return this.observations.get(observationType);
  }

  set(observationType, value) {
    return this.observations.set(observationType, value);
  }

  getBundle() {
    const resources = Array.from(this.observations).map(
      ([observationType, values]) => createObservation(observationType, values)
    );

    return createBundle(resources);
  }

  toJSON() {
    return this.getBundle();
  }
}

// Extend the class with known snomed codes.
Object.entries(snomedCodes).forEach(([snomedCodeKey, snomedCode]) => {
  Object.defineProperty(Observations.prototype, snomedCodeKey, {
    get() {
      return this.get(snomedCode);
    },
    set(value) {
      this.set(snomedCode, value);
    }
  });
});
