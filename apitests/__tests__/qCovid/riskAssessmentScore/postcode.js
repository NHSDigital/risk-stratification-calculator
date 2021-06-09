import { qCovidClient as client } from '../../../helpers/client';
import { CreateBundle } from '../../../helpers/createBundle';
import {
  age,
  sex,
  height,
  weight,
  postcode,
  ethnicGroup,
  housing,
} from '../../../helpers/snomedCodes';

import { Prediction } from '../../../helpers/riskAssessment_Prediction';

describe('The location where a patient lives (dependant on the postcode) will impact the obtained risk score', () => {
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

  it('It should provide a risk score prediction with a warning when no postcode is provided', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 26, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.male.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.blackAfrican.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86, weight.unit);
    const { bundle } = bundleCreator;

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001171, 5.18141592920354);
    expectedPred.addHospitalisation(0.04383, 4.693221972373916);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('When a postcode longer than 9 characters is provided a risk score prediction with a warning is calculated', async () => {
    const bundle = createPatient('wrong12postcode');

    const response = await client.postSuccess(bundle);

    expect(response.contained[bundle.entry.length - 1].issue[0].severity).toBe('warning');
    expect(response.contained[bundle.entry.length - 1].issue[0].details.text).toBe('The provided postcode was not found. A Townsend score of 0 will be used.');

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001171, 5.18141592920354);
    expectedPred.addHospitalisation(0.04383, 4.693221972373916);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('When something that is not a postcode is provided a risk score prediction with a warning is calculated', async () => {
    const bundle = createPatient('wrong12');

    const response = await client.postSuccess(bundle);

    expect(response.contained[bundle.entry.length - 1].issue[0].severity).toBe('warning');
    expect(response.contained[bundle.entry.length - 1].issue[0].details.text).toBe('The provided postcode was not found. A Townsend score of 0 will be used.');

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001171, 5.18141592920354);
    expectedPred.addHospitalisation(0.04383, 4.693221972373916);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction with a warning when half a postcode is provided', async () => {
    const bundle = createPatient('BS1');

    const response = await client.postSuccess(bundle);

    expect(response.contained[bundle.entry.length - 1].issue[0].severity).toBe('warning');
    expect(response.contained[bundle.entry.length - 1].issue[0].details.text).toBe('The provided postcode was not found. A Townsend score of 0 will be used.');

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001171, 5.18141592920354);
    expectedPred.addHospitalisation(0.04383, 4.693221972373916);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for with a warning for a non existing postcode', async () => {
    const bundle = createPatient('bs11nz');

    const response = await client.postSuccess(bundle);

    expect(response.contained[bundle.entry.length - 1].issue[0].severity).toBe('warning');
    expect(response.contained[bundle.entry.length - 1].issue[0].details.text).toBe('The provided postcode was not found. A Townsend score of 0 will be used.');

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001171, 5.18141592920354);
    expectedPred.addHospitalisation(0.04383, 4.693221972373916);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a privileged area', async () => {
    const bundle = createPatient('GU25 4QW');

    const response = await client.postSuccess(bundle);
    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000879, 3.8893805309734515);
    expectedPred.addHospitalisation(0.033482, 3.5851804261698255);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a deprived area', async () => {
    const bundle = createPatient('CO15 2EU');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001412, 6.247787610619469);
    expectedPred.addHospitalisation(0.052236, 5.593318342434951);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a Scottish postcode', async () => {
    const bundle = createPatient('AB39 2TL');

    const response = await client.postSuccess(bundle);
    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001056, 4.672566371681416);
    expectedPred.addHospitalisation(0.039782, 4.259770853410429);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a Welsh postcode', async () => {
    const bundle = createPatient('CF11 9LJ');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.00139, 6.150442477876106);
    expectedPred.addHospitalisation(0.051485, 5.512902880394046);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a Northern Irish postcode', async () => {
    const bundle = createPatient('BT57 8SZ');

    const response = await client.postSuccess(bundle);
    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001122, 4.964601769911504);
    expectedPred.addHospitalisation(0.042093, 4.507227754577578);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a AB12CD style postcodes', async () => {
    const bundle = createPatient('CO152EU');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001412, 6.247787610619469);
    expectedPred.addHospitalisation(0.052236, 5.593318342434951);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a lower case (ab12cd) style postcodes', async () => {
    const bundle = createPatient('co152eu');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001412, 6.247787610619469);
    expectedPred.addHospitalisation(0.052236, 5.593318342434951);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should accept newly created postcodes', async () => {
    const bundle = createPatient('PO7 3DR');

    const response = await client.postSuccess(bundle);
    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001171, 5.18141592920354);
    expectedPred.addHospitalisation(0.04383, 4.693221972373916);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('When a BFPO is provided a risk score prediction with a warning is calculated', async () => {
    const bundle = createPatient('BF1 1AE');

    const response = await client.postSuccess(bundle);

    expect(response.contained[bundle.entry.length - 1].issue[0].severity).toBe('warning');
    expect(response.contained[bundle.entry.length - 1].issue[0].details.text).toBe('The provided postcode was not found. A Townsend score of 0 will be used.');

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001171, 5.18141592920354);
    expectedPred.addHospitalisation(0.04383, 4.693221972373916);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('When a British Force based in the UK is provided a risk score prediction is calculated', async () => {
    const bundle = createPatient('TR12 7AU');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001183, 5.234513274336283);
    expectedPred.addHospitalisation(0.044233, 4.736374344148196);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a prison postcode', async () => {
    const bundle = createPatient('BS7 8PS');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001044, 4.6194690265486723);
    expectedPred.addHospitalisation(0.03936, 4.214584002569868);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a postcode that covers 7 streets', async () => {
    const bundle = createPatient('HD7 5UZ');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000866, 3.831858407079646);
    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    expectedPred.addHospitalisation(0.033, 3.5335689045936394);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for a London style postcode', async () => {
    const bundle = createPatient('EC1A 1AA');

    const response = await client.postSuccess(bundle);

    expect(response.contained).toHaveLength(bundle.entry.length);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.00192, 8.495575221238939);
    expectedPred.addHospitalisation(0.069695, 7.462790448656173);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for trailing spaces \'ab12cd    \' style postcodes', async () => {
    const bundle = createPatient('co152eu    ');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001412, 6.247787610619469);
    expectedPred.addHospitalisation(0.052236, 5.593318342434951);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('It should provide a risk score prediction for leading spaces \'   ab12cd\' style postcodes', async () => {
    const bundle = createPatient('    co152eu');

    const response = await client.postSuccess(bundle);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.001412, 6.247787610619469);
    expectedPred.addHospitalisation(0.052236, 5.593318342434951);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });
});
