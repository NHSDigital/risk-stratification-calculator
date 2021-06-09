export class CreateBundle {
  constructor() {
    this.bundle = {
      resourceType: 'Bundle',
      type: 'transaction',
      entry: []
    };
  }

  addObservationValueQuantity(id, code, value, unit) {
    const observation = {
      resource: {
        resourceType: 'Observation',
        id,
        code: {
          coding: [
            {
              system: this.getSystem(code),
              code
            }
          ]
        },
        valueQuantity: {
          value,
          unit,
          system: 'http://unitsofmeasure.org',
          code: unit
        }
      }
    };
    this.bundle.entry.push(observation);
  }

  addObservationValueString(id, code, value) {
    const observation = {
      resource: {
        resourceType: 'Observation',
        id,
        code: {
          coding: [
            {
              system: this.getSystem(code),
              code
            }
          ]
        },
        valueString: value
      }
    };
    this.bundle.entry.push(observation);
  }

  addObservationValueBoolean(id, code) {
    const observation = {
      resource: {
        resourceType: 'Observation',
        id,
        code: {
          coding: [
            {
              system: this.getSystem(code),
              code
            }
          ]
        },
        valueBoolean: true
      }
    };
    this.bundle.entry.push(observation);
  }

  addObservationValueCodeableConcept(id, code1, code2) {
    const observation = {
      resource: {
        resourceType: 'Observation',
        id,
        code: {
          coding: [
            {
              system: this.getSystem(code1),
              code: code1
            }
          ]
        },
        valueCodeableConcept: {
          coding: [
            {
              system: this.getSystem(code2),
              code: code2
            }
          ]
        }
      }
    };
    this.bundle.entry.push(observation);
  }

  addObservationWrongJsonFormat(id, code, value) {
    const observation = {
      resource: {
        id,
        code: {
          coding: [
            {
              system: 'http://snomed.info/sct',
              code
            }
          ]
        },
        valueBoolean: value
      }
    };
    this.bundle.entry.push(observation);
  }

  getSystem(code) {
    if (code.match('[a-zA-Z]')) { return 'http://nhsd.riskstrat.nhs/qcovid'; }
    return 'http://snomed.info/sct';
  }
}
