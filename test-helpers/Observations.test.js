import { Observations } from './Observations';
import { age } from './snomedCodes';

test('Can set observations using properties', () => {
  const observations = new Observations();

  observations.age = 34;

  expect(observations.age).toBe(34);
});

test('Can add codeable concept value', () => {
  const observations = new Observations();

  observations.set(age, 67);

  const bundle = observations.getBundle();

  expect(bundle.entry).toMatchObject([{
    resource: {
      resourceType: 'Observation',
      id: age.id,
      code: {
        coding: [{
          system: 'http://snomed.info/sct',
          code: age.snomedCode,
        }]
      },
      valueQuantity: {
        value: 67,
        unit: 'years',
        system: 'http://unitsofmeasure.org',
        code: 'years',
      },
    }
  }]);
});
