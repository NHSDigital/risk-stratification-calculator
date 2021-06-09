import { qCovidClient as client } from '../../../helpers/client';
import { CreateBundle } from '../../../helpers/createBundle';
import {
  age, sex, height, weight, ethnicGroup
} from '../../../helpers/snomedCodes';
import * as OperationOutcomeDiagnostics from '../../../helpers/observationOutcomeDiagnostics';

describe('When wrong values for age are sent then it should get an Operation Outcome', () => {
  it('It should fail to calculate a risk score when an underage is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 18, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.issue[0].diagnostics).toContain(OperationOutcomeDiagnostics.WRONG_AGE_VALUE);
    expect(response.issue[0].severity).toBe('error');
  });

  it('It should fail to calculate a risk score when over 100 years is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 101, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.issue[0].diagnostics).toContain(OperationOutcomeDiagnostics.WRONG_AGE_VALUE);
    expect(response.issue[0].severity).toBe('error');
  });

  it('It should not fail to calculate a risk score when 19yo is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 19, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should not fail to calculate a risk score when 100yo is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 100, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should fail to calculate a risk score when wrong data type is sent for age', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 'abc', age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 145, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.message).toContain(OperationOutcomeDiagnostics.INVALID_REQUEST_BODY);
  });

  it('It should fail to calculate a risk score when a negative age value is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, -20, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.issue[0].diagnostics).toContain(OperationOutcomeDiagnostics.WRONG_AGE_VALUE);
    expect(response.issue[0].severity).toBe('error');
  });

  it('It should fail to calculate a risk score when an empty age value is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, null, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.message).toContain(OperationOutcomeDiagnostics.INVALID_REQUEST_BODY);
  });
});
