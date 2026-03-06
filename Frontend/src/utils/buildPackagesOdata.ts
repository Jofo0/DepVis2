type Filters = {
  ecosystem?: string | null;
  vulnerability?: string | null;
  depth?: string | null;
};

const escapeODataLiteral = (v: string) => v.replace(/'/g, "''");

const joinAnd = (parts: string[]) => parts.join("+and+");

export const buildPackagesOdata = ({
  ecosystem,
  vulnerability,
  depth,
}: Filters) => {
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

  if (depth) {
    const d = depth.toLowerCase();

    if (d === "direct") {
      preds.push(`depth+eq+2`);
    } else if (d === "transitive") {
      preds.push(`depth+gt+2`);
    } else if (d === "manifests") {
      preds.push(`depth+eq+1+or+depth+eq+0`);
    }
  }

  return preds.length ? `${joinAnd(preds)}` : "";
};
