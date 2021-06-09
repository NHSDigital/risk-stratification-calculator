import { qCovidClient as client } from '../../../helpers/client';
import { CreateBundle } from '../../../helpers/createBundle';
import {
  age, sex, height, weight, ethnicGroup, postcode, housing
} from '../../../helpers/snomedCodes';
import * as OperationOutcomeDiagnostics from '../../../helpers/observationOutcomeDiagnostics';

describe('When wrong postcode values are sent then it should get an Operation Outcome', () => {
  function createPatient(patientsPostcode) {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 26, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.male.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.blackAfrican.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, patientsPostcode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86, weight.unit);
    return bundleCreator.bundle;
  }

  it('It should provide a risk score prediction with a warning when several spaces as a postcode are provided only', async () => {
    const bundle = createPatient('     ');

    const response = await client.postError(bundle);
    expect(response.issue[0].diagnostics)
      .toContain(OperationOutcomeDiagnostics.MUST_HAVE_A_VALUE_POSTCODE);
    expect(response.issue[0].severity).toBe('error');
  });
});
