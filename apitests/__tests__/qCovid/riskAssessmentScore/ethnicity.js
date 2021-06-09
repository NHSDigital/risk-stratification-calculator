import { qCovidClient as client } from '../../../helpers/client';
import { CreateBundle } from '../../../helpers/createBundle';
import {
  age,
  sex,
  ethnicGroup,
  height,
  weight,
  housing,
} from '../../../helpers/snomedCodes';
import { Prediction } from '../../../helpers/riskAssessment_Prediction';

describe('The person\'s ethnicity will have an impact on the risk score', () => {
  function createPatient(ethnicity) {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 26, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.male.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicity);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 160, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 80, weight.unit);
    return bundleCreator.bundle;
  }

  it('Score for NOT_STATED ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.notStated.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000409, 1.8097345132743363);
    expectedPred.addHospitalisation(0.01813, 1.9413213406146268);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Black african ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.blackAfrican.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.00124, 5.486725663716814);
    expectedPred.addHospitalisation(0.047014, 5.034157832744405);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Black African and White ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.blackAfricanAndWhite.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000833, 3.685840707964602);
    expectedPred.addHospitalisation(0.038412, 4.113074204946996);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Black Caribbean ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.blackCaribbean.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000844, 3.734513274336283);
    expectedPred.addHospitalisation(0.041427, 4.435913909412142);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Black Caribbean and White ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.blackCaribbeanAndWhite.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000833, 3.685840707964602);
    expectedPred.addHospitalisation(0.038412, 4.113074204946996);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Black other ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.blackOther.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000833, 3.685840707964602);
    expectedPred.addHospitalisation(0.038412, 4.113074204946996);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Bangladeshi ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000929, 4.110619469026549);
    expectedPred.addHospitalisation(0.031019, 3.32144769247242743);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Chinese ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.asianChinese.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.00101, 4.469026548672566);
    expectedPred.addHospitalisation(0.027306, 2.923867651782846);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Indian ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.asianIndian.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000649, 2.8716814159292037);
    expectedPred.addHospitalisation(0.038896, 4.16489988221437);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Pakistani ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.asianPakistani.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000754, 3.336283185840708);
    expectedPred.addHospitalisation(0.036521, 3.9105899989292214);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Mixed Asian and White ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.mixedAsianAndWhite.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000833, 3.685840707964602);
    expectedPred.addHospitalisation(0.038412, 4.113074204946996);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Asian Other ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.asianOther.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000825, 3.650442477876106);
    expectedPred.addHospitalisation(0.04146, 4.439447478316736);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Mixed Other Backgrounds ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.mixedOtherBackground.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000833, 3.685840707964602);
    expectedPred.addHospitalisation(0.038412, 4.113074204946996);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for British ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.whiteBritish.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000409, 1.8097345132743363);
    expectedPred.addHospitalisation(0.01813, 1.9413213406146268);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Irish ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.whiteIrish.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000409, 1.8097345132743363);
    expectedPred.addHospitalisation(0.01813, 1.9413213406146268);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for White Other ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.whiteOther.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000409, 1.8097345132743363);
    expectedPred.addHospitalisation(0.01813, 1.9413213406146268);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for Any other group ethnicity', async () => {
    const bundle = createPatient(ethnicGroup.anyOtherGroup.snomedCode);

    const response = await client.postSuccess(bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000833, 3.685840707964602);
    expectedPred.addHospitalisation(0.038412, 4.113074204946996);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });
});
