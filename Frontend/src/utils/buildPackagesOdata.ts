type Filters = {
  ecosystem?: string | null;
  vulnerability?: string | null;
};

const escapeODataLiteral = (v: string) => v.replace(/'/g, "''");

const joinAnd = (parts: string[]) => parts.join("+and+");

export const buildPackagesOdata = ({ ecosystem, vulnerability }: Filters) => {
  const preds: string[] = [];

  if (ecosystem) {
    preds.push(`ecosystem+eq+'${escapeODataLiteral(ecosystem)}'`);
  }

  if (vulnerability) {
    const v = vulnerability.toLowerCase();
    if (v === "vulnerable") {
      preds.push("Vulnerabilities/$count+gt+0");
    } else if (v === "ok") {
      preds.push("Vulnerabilities/$count+eq+0");
    }
  }

  return preds.length ? `${joinAnd(preds)}` : "";
};
