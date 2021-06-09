import { qCovidClient as client } from '../../../helpers/client';
import { CreateBundle } from '../../../helpers/createBundle';
import {
  age, sex, height, weight, ethnicGroup
} from '../../../helpers/snomedCodes';
import * as OperationOutcomeDiagnostics from '../../../helpers/observationOutcomeDiagnostics';

describe('Fails to calculate an score when out of bounds values are provided for the bmi calculation', () => {
  it('It should not fail to calculate a risk score when a height below 140 is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 20, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 139, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should not fail to calculate a risk score when a height over 210 is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 21, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 211, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should not fail to calculate a risk score when a weight below 40kgs is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 21, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 39, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should not fail to calculate a risk score when a weight over 180kgs is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 21, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 181, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should fail to calculate a risk score when wrong data type is sent for height', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 22, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 'abc', height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.message).toContain(OperationOutcomeDiagnostics.INVALID_REQUEST_BODY);
  });

  it('It should fail to calculate a risk score when wrong data type is sent for weight', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 22, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 180, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 'abc', weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.message).toContain(OperationOutcomeDiagnostics.INVALID_REQUEST_BODY);
  });

  it('It should not fail to calculate a risk score when bmi is below 15', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 20, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 164, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 39, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should not fail to calculate a risk score when bmi is over 47', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 20, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 164, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 200, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should not fail to calculate a risk score when bmi is over 15', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 20, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 164, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 41, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should not fail to calculate a risk score when bmi is below 47', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 21, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 164, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 126, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    await client.postSuccess(bundleCreator.bundle);
  });

  it('It should not fail to calculate a risk score when no height nor weight are provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 21, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postSuccess(bundleCreator.bundle);
    expect(response.contained[bundleCreator.bundle.entry.length].issue[0].severity).toBe('warning');
    expect(response.contained[bundleCreator.bundle.entry.length].issue[0].details.text).toBe('No BMI was provided. A BMI of 25 will be used.');
  });
});
