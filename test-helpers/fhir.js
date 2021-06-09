export function createBundle(resources) {
  return {
    resourceType: 'Bundle',
    type: 'transaction',
    entry: resources.map((resource) => ({ resource }))
  };
}

export function createObservation(observationType, value) {
  const { id, snomedCode: code } = observationType;

  const resource = {
    resourceType: 'Observation',
    id,
    code: {
      coding: [
        {
          system: 'http://snomed.info/sct',
          code
        }
      ]
    },
  };

  if (typeof observationType.unit === 'string') {
    const { unit } = observationType;

    resource.valueQuantity = {
      value,
      unit,
      system: 'http://unitsofmeasure.org',
      code: unit
    };
  } else if (typeof value.snomedCode === 'string') {
    resource.valueCodeableConcept = {
      coding: [
        {
          system: 'http://snomed.info/sct',
          code: value.snomedCode
        }
      ]
    };
  } else {
    switch (typeof value) {
      case 'string':
        resource.valueString = value;
        break;
      case 'boolean':
        resource.valueBoolean = value;
        break;
      default:
        break;
    }
  }

  return resource;
}
